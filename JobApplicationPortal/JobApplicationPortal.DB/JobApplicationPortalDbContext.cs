using JobApplicationPortal.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.DB
{
    public class JobApplicationPortalDbContext : DbContext
    {
        public JobApplicationPortalDbContext(DbContextOptions<JobApplicationPortalDbContext> options)
            : base(options)
        {}

        public DbSet<Applicant> Applicants { get; set; }
    }
}
