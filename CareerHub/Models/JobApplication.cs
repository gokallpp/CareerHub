using System.ComponentModel.DataAnnotations.Schema;

namespace CareerHub.Models
{
    [Table("job_applications")]
    public class JobApplication
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("candidate_id")]
        public string CandidateId { get; set; }

        [Column("job_posting_id")]
        public int JobPostingId { get; set; }

        [Column("applied_date")]
        public DateTime AppliedDate { get; set; }

        [Column("status")]
        public string Status { get; set; }

        public ApplicationUser Candidate { get; set; }

        public JobPosting JobPosting { get; set; }
    }
}
