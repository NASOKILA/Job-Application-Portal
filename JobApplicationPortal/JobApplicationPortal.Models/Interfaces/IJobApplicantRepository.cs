using JobApplicationPortal.Models.DbModels;

namespace JobApplicationPortal.Models.Interfaces
{
    public interface IJobApplicantsRepository
    {
        Task<JobApplicants> AddJobApplicantAsync(JobApplicants jobApplicant);
        Task<JobApplicants> GetJobApplicantByUniqueIdAsync(string uniqueId);
        Task<List<JobApplicants>> GetAllJobApplicantsAsync();
        Task SaveChangesAsync();
    }
}
