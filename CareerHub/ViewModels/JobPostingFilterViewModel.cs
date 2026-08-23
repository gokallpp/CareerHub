using CareerHub.Models;

namespace CareerHub.ViewModels
{
    public class JobPostingFilterViewModel
    {
        public string? SearchTerm { get; set; }

        public string? Location { get; set; }

        public string? JobType { get; set; }

        public string? WorkType { get; set; }

        public List<JobPosting> JobPostings { get; set; } = new();


        // Pagination
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalJobCount { get; set; }
    }
}
