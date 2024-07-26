using JobApplicationPortal.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Pinewood.Customers.Db
{
    public class JobApplicationPortalDbContext : DbContext
    {
        public JobApplicationPortalDbContext(DbContextOptions<JobApplicationPortalDbContext> options)
            : base(options)
        {}

        public DbSet<Applicant> Applicants { get; set; }
    }
}
