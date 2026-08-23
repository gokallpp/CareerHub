using CareerHub.Data;
using CareerHub.Models;
using CareerHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public EmployerController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return RedirectToAction(
                    "Create",
                    "Companies"
                );
            }

            var jobPostings = await _context.JobPostings
                .Where(x => x.CompanyId == company.Id)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking()
                .ToListAsync();

            var viewModel = new EmployerDashboardViewModel
            {
                Company = company,

                TotalJobPostings = jobPostings.Count,

                ActiveJobPostings =
                    jobPostings.Count(x => x.IsActive),

                JobPostings = jobPostings
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> CreateJobPosting()
        {
            var userId = _userManager.GetUserId(User);

            var hasCompany = await _context.Companies
                .AnyAsync(x => x.OwnerId == userId);

            if (!hasCompany)
            {
                return RedirectToAction(
                    "Create",
                    "Companies"
                );
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJobPosting(
    JobPostingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return RedirectToAction(
                    "Create",
                    "Companies"
                );
            }

            var jobPosting = new JobPosting
            {
                Title = model.Title,
                JobType = model.JobType,
                WorkType = model.WorkType,
                Description = model.Description,
                Location = model.Location,
                Salary = model.Salary,

                CreatedDate = DateTime.UtcNow,
                IsActive = true,

                CompanyId = company.Id
            };

            _context.JobPostings.Add(jobPosting);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> EditJobPosting(int id)
        {
            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return NotFound();
            }

            var job = await _context.JobPostings
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == company.Id);

            if (job == null)
            {
                return NotFound();
            }

            var model = new JobPostingEditViewModel
            {
                Id = job.Id,
                Title = job.Title,
                JobType = job.JobType,
                WorkType = job.WorkType,
                Description = job.Description,
                Location = job.Location,
                Salary = job.Salary
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditJobPosting(
    JobPostingEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return NotFound();
            }

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id &&
                    x.CompanyId == company.Id);

            if (job == null)
            {
                return NotFound();
            }

            job.Title = model.Title;
            job.JobType = model.JobType;
            job.WorkType = model.WorkType;
            job.Description = model.Description;
            job.Location = model.Location;
            job.Salary = model.Salary;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleJobPostingStatus(int id)
        {
            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return NotFound();
            }

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == company.Id);

            if (job == null)
            {
                return NotFound();
            }

            job.IsActive = !job.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Giriş yapan işverenin şirketine ait ilanlara yapılan başvuruları,
        // aday ve iş ilanı bilgileriyle birlikte listeler.
        public async Task<IActionResult> Applications()
        {
            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return RedirectToAction(
                    "Create",
                    "Companies"
                );
            }

            var applications = await _context.JobApplications
                .Where(x => x.JobPosting.CompanyId == company.Id)
                .Include(x => x.Candidate)
                .Include(x => x.JobPosting)
                .OrderByDescending(x => x.AppliedDate)
                .AsNoTracking()
                .ToListAsync();

            return View(applications);
        }


        // Giriş yapan işverenin kendi ilanına ait bir başvurunun
        // durumunu güvenli bir şekilde günceller.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateApplicationStatus(
            int id,
            string status)
        {
            var allowedStatuses = new[]
            {
        "Pending",
        "Reviewed",
        "Interview",
        "Accepted",
        "Rejected"
    };

            if (!allowedStatuses.Contains(status))
            {
                return BadRequest();
            }

            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return NotFound();
            }

            var application = await _context.JobApplications
                .Include(x => x.JobPosting)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobPosting.CompanyId == company.Id);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = status;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Applications));
        }



        // İşverenin yalnızca kendi şirketinin ilanına başvuran
        // adayın CV dosyasını görüntülemesini sağlar.
        public async Task<IActionResult> ViewCv(int id)
        {
            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return NotFound();
            }

            var application = await _context.JobApplications
                .Include(x => x.Candidate)
                .Include(x => x.JobPosting)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.JobPosting.CompanyId == company.Id);

            if (application == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(
                application.Candidate.CvStoredFileName))
            {
                return NotFound();
            }

            var storedFileName = Path.GetFileName(
                application.Candidate.CvStoredFileName);

            var filePath = Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "CVs",
                storedFileName
            );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(
                filePath,
                "application/pdf"
            );
        }
    }
}