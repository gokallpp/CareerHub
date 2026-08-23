using CareerHub.Data;
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerHub.ViewModels;

namespace CareerHub.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobPostingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Aktif iş ilanlarını arama, filtreleme ve sayfalama
        // kriterlerine göre getirir.
        public async Task<IActionResult> Index(
            JobPostingFilterViewModel model,
            int page = 1)
        {
            const int pageSize = 6;

            if (page < 1)
            {
                page = 1;
            }

            var query = _context.JobPostings
                .Where(x => x.IsActive)
                .Include(x => x.Company)           
                .AsNoTracking()
                .AsQueryable();


            // Pozisyon / açıklama araması
            if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                var searchTerm = model.SearchTerm.Trim();

                query = query.Where(x =>
                    EF.Functions.ILike(
                        x.Title,
                        $"%{searchTerm}%") ||
                    EF.Functions.ILike(
                        x.Description,
                        $"%{searchTerm}%"));
            }


            // Şehir filtresi
            if (!string.IsNullOrWhiteSpace(model.Location))
            {
                query = query.Where(x =>
                    x.Location == model.Location);
            }


            // İş tipi
            if (!string.IsNullOrWhiteSpace(model.JobType))
            {
                query = query.Where(x =>
                    x.JobType == model.JobType);
            }


            // Çalışma şekli
            if (!string.IsNullOrWhiteSpace(model.WorkType))
            {
                query = query.Where(x =>
                    x.WorkType == model.WorkType);
            }


            // Filtrelenmiş toplam ilan sayısı
            var totalJobCount = await query.CountAsync();

            // Toplam sayfa sayısı
            var totalPages = (int)Math.Ceiling(
                totalJobCount / (double)pageSize
            );


            // İlanları sayfalayarak getir
            model.JobPostings = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            model.CurrentPage = page;
            model.TotalPages = totalPages;
            model.TotalJobCount = totalJobCount;


            return View(model);
        }



        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPostings
                .Include(x => x.Company)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }
    }
}

        
