using CareerHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CareerHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobPosting> JobPostings { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<JobApplication> JobApplications { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Company>()
                .HasOne(x => x.Owner)
                .WithOne()
                .HasForeignKey<Company>(x => x.OwnerId)
                .IsRequired(false);


            builder.Entity<JobApplication>()
                .HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId);


            builder.Entity<JobApplication>()
                .HasOne(x => x.JobPosting)
                .WithMany()
                .HasForeignKey(x => x.JobPostingId);


            builder.Entity<JobApplication>()
                .HasIndex(x => new
                {
                    x.CandidateId,
                    x.JobPostingId
                })
                .IsUnique();
        }
    }
}
