using CareerHub.Models;

namespace CareerHub.ViewModels
{
    public class EmployerDashboardViewModel
    {
        public Company Company { get; set; } 

        public int TotalJobPostings { get; set; }

        public int ActiveJobPostings { get; set; }

        public List<JobPosting> JobPostings { get; set; } = new();
    }
}
