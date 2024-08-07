using AutoMapper;
using JobApplicationPortal.DB;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;
using JobApplicationPortal.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationPortal.Backend.Services
{
    public class JobApplicantService
    {
        private readonly JobApplicationPortalDbContext _context;
        private readonly IBlobService _blobService;
        private readonly IMapper _mapper;
        private readonly ILogger<JobApplicantService> _logger;

        public JobApplicantService(JobApplicationPortalDbContext context, IBlobService blobService, IMapper mapper, ILogger<JobApplicantService> logger)
        {
            _context = context;
            _blobService = blobService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<JobApplicants> AddJobApplicantAsync(JobApplicantDto model)
        {
            _logger.LogInformation("Mapping job applicant DTO to entity.");
            var jobApplicant = _mapper.Map<JobApplicants>(model);
            jobApplicant.Id = Guid.NewGuid();
            var containerName = jobApplicant.Id.ToString();

            _logger.LogInformation("Uploading resume for job applicant {Id}.", jobApplicant.Id);
            if (model.Resume != null)
            {
                var resumeFileName = $"Resumes/{model.Resume.FileName}";
                await _blobService.UploadFileBlobAsync(model.Resume, containerName, resumeFileName);
                jobApplicant.ResumeFileName = model.Resume.FileName;
            }


            _logger.LogInformation("Uploading certifications for job applicant {Id}.", jobApplicant.Id);
            if (model.Certifications != null)
            {
                jobApplicant.CertificationsFilesNames = new List<string>();
                foreach (var certification in model.Certifications)
                {
                    var certificationFileName = $"Certifications/{certification.FileName}";
                    await _blobService.UploadFileBlobAsync(certification, containerName, certificationFileName);
                    jobApplicant.CertificationsFilesNames.Add(certification.FileName);
                }
            }


            _logger.LogInformation("Adding job applicant {Id} to the database.", jobApplicant.Id);
            await _context.JobApplicants.AddAsync(jobApplicant);
            await _context.SaveChangesAsync();


            _logger.LogInformation("Job applicant {Id} added successfully.", jobApplicant.Id);
            return jobApplicant;
        }

        public async Task<JobApplicants> GetJobApplicantByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching job applicant by ID {Id}.", id);
            return await _context.JobApplicants.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<JobApplicants>> GetAllJobApplicantsAsync()
        {
            _logger.LogInformation("Fetching all job applicants.");
            return await _context.JobApplicants.ToListAsync();
        }
    }

}
