using JobApplicationPortal.DB;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.Repositories
{
    public class JobApplicantsRepository : IJobApplicantsRepository
    {
        private readonly JobApplicationPortalDbContext _context;

        public JobApplicantsRepository(JobApplicationPortalDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplicants> AddJobApplicantAsync(JobApplicants jobApplicant)
        {
            _context.JobApplicants.Add(jobApplicant);
            return jobApplicant;
        }

        public async Task<JobApplicants> GetJobApplicantByUniqueIdAsync(string uniqueId)
        {
            return await _context.JobApplicants.FirstOrDefaultAsync(x => x.UniqueId == uniqueId);
        }

        public async Task<List<JobApplicants>> GetAllJobApplicantsAsync()
        {
            return await _context.JobApplicants.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
