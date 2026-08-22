using CareerHub.Data;
using CareerHub.Models;
using CareerHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompaniesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies
                .AsNoTracking()
                .ToListAsync();

            return View(companies);
        }


        public async Task<IActionResult> Details(int id)
        {
            var company = await _context.Companies
                .Include(x => x.JobPostings) //Şirketi getirirken ona bağlı iş ilanlarını da getir.
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }


        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> MyCompany()
        {
            var userId = _userManager.GetUserId(User);

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId);

            if (company == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return RedirectToAction(
                nameof(Details),
                new { id = company.Id }
            );
        }

        [Authorize(Roles = "Employer")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);

            var hasCompany = await _context.Companies
                .AnyAsync(x => x.OwnerId == userId);

            if (hasCompany)
            {
                return RedirectToAction(nameof(MyCompany));
            }

            return View();
        }


        [Authorize(Roles = "Employer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    CompanyCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            var hasCompany = await _context.Companies
                .AnyAsync(x => x.OwnerId == userId);

            if (hasCompany)
            {
                return RedirectToAction(nameof(MyCompany));
            }

            var company = new Company
            {
                Name = model.Name,
                Description = model.Description,
                Website = model.Website,

                // Giriş yapan işverenin ID'si
                OwnerId = userId
            };

            _context.Companies.Add(company);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Details),
                new { id = company.Id }
            );
        }
    }
}
