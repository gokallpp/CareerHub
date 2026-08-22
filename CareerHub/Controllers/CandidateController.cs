using CareerHub.Data;
using CareerHub.Models;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
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
                    x.CandidateId == userId &&
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
                CandidateId = userId,
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
    }
}
