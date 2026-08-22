using System.ComponentModel.DataAnnotations.Schema;

namespace CareerHub.Models
{
    [Table("job_postings")]
    public class JobPosting
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("job_type")]
        public string JobType { get; set; }

        [Column("work_type")]
        public string WorkType { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("location")]
        public string Location { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("salary")]
        public decimal Salary { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("company_id")]
        public int? CompanyId { get; set; }

        public Company? Company { get; set; }
    }
}
