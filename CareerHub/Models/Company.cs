using System.ComponentModel.DataAnnotations.Schema;

namespace CareerHub.Models
{
    [Table("companies")]
    public class Company
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("website")]
        public string? Website { get; set; }

        [Column("owner_id")]
        public string? OwnerId { get; set; }

        public ApplicationUser? Owner { get; set; }

        public List<JobPosting> JobPostings { get; set; } = new();
    }
}
