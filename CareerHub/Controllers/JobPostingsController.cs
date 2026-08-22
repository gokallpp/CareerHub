using CareerHub.Data;
using CareerHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    public class JobPostingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobPostingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var jobPostings = await _context.JobPostings
                .Where(x => x.IsActive)
                .Include(x => x.Company)//İş ilanlarını getirirken bağlı oldukları şirketleri de getir.
                .AsNoTracking()
                .ToListAsync();

            return View(jobPostings);
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

        
