using CareerHub.Data;
using CareerHub.Models;
using CareerHub.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var candidates = await _userManager
                .GetUsersInRoleAsync("Candidate");

            var employers = await _userManager
                .GetUsersInRoleAsync("Employer");

            var model = new AdminDashboardViewModel
            {
                CandidateCount = candidates.Count,
                EmployerCount = employers.Count,

                CompanyCount = await _context.Companies
                    .AsNoTracking()
                    .CountAsync(),

                JobPostingCount = await _context.JobPostings
                    .AsNoTracking()
                    .CountAsync(),

                ApplicationCount = await _context.JobApplications
                    .AsNoTracking()
                    .CountAsync()
            };

            return View(model);
        }
    }
}
