using CareerHub.Data;
using CareerHub.Models;
using CareerHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CareerHub.Controllers
{
    [Authorize(Roles = "Candidate")]
    public class CandidateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CandidateController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Giriş yapan adayın aktif bir iş ilanına başvurmasını sağlar.
        // Adayın CV'si yoksa başvuruya izin vermez.
        // Aynı ilana ikinci kez başvurmayı engeller.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            // Adayın CV'si yoksa başvuru yapmasına izin verme.
            if (string.IsNullOrWhiteSpace(user.CvStoredFileName))
            {
                TempData["ApplicationMessage"] =
                    "Başvuru yapmadan önce CV yüklemelisiniz.";

                return RedirectToAction(
                    "Details",
                    "JobPostings",
                    new { id }
                );
            }

            var job = await _context.JobPostings
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.IsActive);

            if (job == null)
            {
                return NotFound();
            }

            var alreadyApplied = await _context.JobApplications
                .AnyAsync(x =>
                    x.CandidateId == user.Id &&
                    x.JobPostingId == id);

            if (alreadyApplied)
            {
                TempData["ApplicationMessage"] =
                    "Bu ilana daha önce başvurdunuz.";

                return RedirectToAction(
                    "Details",
                    "JobPostings",
                    new { id }
                );
            }

            var application = new JobApplication
            {
                CandidateId = user.Id,
                JobPostingId = job.Id,
                AppliedDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.JobApplications.Add(application);

            await _context.SaveChangesAsync();

            TempData["ApplicationMessage"] =
                "Başvurunuz başarıyla alındı.";

            return RedirectToAction(
                "Details",
                "JobPostings",
                new { id }
            );
        }


        // Giriş yapan adayın yaptığı tüm iş başvurularını getirir.
        // Başvurularla birlikte ilgili iş ilanını ve şirket bilgisini de yükler.
        // Başvuruları en yeni başvuru en üstte olacak şekilde sıralar
        // ve sonuçları MyApplications View'ına gönderir.
        // Giriş yapan adayın başvurularını, ilan ve şirket bilgileriyle birlikte listeler.
        public async Task<IActionResult> MyApplications()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var applications = await _context.JobApplications
                .Where(x => x.CandidateId == userId)
                .Include(x => x.JobPosting)
                    .ThenInclude(x => x.Company)
                .OrderByDescending(x => x.AppliedDate)
                .AsNoTracking()
                .ToListAsync();

            return View(applications);
        }



        // Giriş yapan adayın CV yükleme sayfasını gösterir.
        [HttpGet]
        public async Task<IActionResult> Cv()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            ViewBag.CurrentCvFileName = user.CvFileName;

            return View();
        }

        // Giriş yapan adayın PDF formatındaki CV dosyasını güvenli klasöre kaydeder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cv(CvUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var file = model.CvFile;

            // Maksimum 5 MB
            const long maxFileSize = 5 * 1024 * 1024;

            if (file.Length == 0)
            {
                ModelState.AddModelError(
                    "CvFile",
                    "Dosya boş olamaz.");

                return View(model);
            }

            if (file.Length > maxFileSize)
            {
                ModelState.AddModelError(
                    "CvFile",
                    "CV dosyası en fazla 5 MB olabilir.");

                return View(model);
            }

            var extension = Path.GetExtension(file.FileName);

            if (!string.Equals(
                extension,
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    "CvFile",
                    "Sadece PDF dosyası yükleyebilirsiniz.");

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var cvFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "App_Data",
                "CVs"
            );

            Directory.CreateDirectory(cvFolder);

            // Gerçek dosya adını sunucuda kullanmıyoruz.
            var storedFileName =
                $"{Guid.NewGuid():N}.pdf";

            var filePath = Path.Combine(
                cvFolder,
                storedFileName
            );

            await using (var stream =
                new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Kullanıcının eski CV'si varsa silelim.
            if (!string.IsNullOrEmpty(user.CvStoredFileName))
            {
                var oldFilePath = Path.Combine(
                    cvFolder,
                    user.CvStoredFileName
                );

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            user.CvFileName =
                Path.GetFileName(file.FileName);

            user.CvStoredFileName =
                storedFileName;

            await _userManager.UpdateAsync(user);

            TempData["CvMessage"] =
                "CV'niz başarıyla yüklendi.";

            return RedirectToAction(nameof(Cv));
        }

        // Giriş yapan adayın profil bilgilerini gösterir.
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var model = new CandidateProfileViewModel
            {
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                AboutMe = user.AboutMe,
                CvFileName = user.CvFileName
            };

            return View(model);
        }


        // Giriş yapan adayın profil bilgilerini günceller.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(
            CandidateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.City = model.City;
            user.AboutMe = model.AboutMe;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description
                    );
                }

                return View(model);
            }

            TempData["ProfileMessage"] =
                "Profil bilgileriniz başarıyla güncellendi.";

            return RedirectToAction(nameof(Profile));
        }


    }
}
