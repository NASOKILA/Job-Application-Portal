using AutoMapper;
using FluentValidation;
using JobApplicationPortal.Backend.Responses;
using JobApplicationPortal.Models.DbModels;
using JobApplicationPortal.Models.DTOModels;
using JobApplicationPortal.Models.Interfaces;
using JobApplicationPortal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace JobApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicantsController : ControllerBase
    {
        private readonly IJobApplicantsRepository _repository;
        private readonly IBlobService _blobService;
        private readonly IMapper _mapper;
        private readonly IValidator<JobApplicantDto> _validator;
        private readonly ILogger<JobApplicantsController> _logger;

        public JobApplicantsController(IJobApplicantsRepository repository, IBlobService blobService, IMapper mapper, IValidator<JobApplicantDto> validator, ILogger<JobApplicantsController> logger)
        {
            _validator = validator;
            _repository = repository;
            _blobService = blobService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromForm] JobApplicantDto model)
        {
            try
            {
                _logger.LogInformation("Starting validation for job applicant submission.");
                var result = await _validator.ValidateAsync(model);
            
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(e => new ValidationError
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    }).ToList();

                    _logger.LogWarning("Validation failed for job applicant submission: {Errors}", errors);
                    return BadRequest(new JobApplicationResponse() { Success = false, Message = "Validation errors occurred.", Errors = errors });
                }

                var jobApplicant = _mapper.Map<JobApplicants>(model);

                jobApplicant.UniqueId = Guid.NewGuid().ToString();
                var containerName = jobApplicant.UniqueId;

                _logger.LogInformation("Uploading resume for job applicant {UniqueId}.", jobApplicant.UniqueId);
                if (model.Resume != null)
                {
                    var resumeFileName = $"Resumes/{model.Resume.FileName}";
                    await _blobService.UploadFileBlobAsync(model.Resume, containerName, resumeFileName);
                    jobApplicant.ResumeFileName = model.Resume.FileName;
                }

                _logger.LogInformation("Uploading certifications for job applicant {UniqueId}.", jobApplicant.UniqueId);
                if (model.Certifications != null)
                {
                    jobApplicant.CertificationsFilesNames = new List<string>();
                    foreach (var certification in model.Certifications)
                    {
                        var resumeFileName = $"Certifications/{certification.FileName}";
                        await _blobService.UploadFileBlobAsync(certification, containerName, resumeFileName);
                        jobApplicant.CertificationsFilesNames.Add(certification.FileName);
                    }
                }

                await _repository.AddJobApplicantAsync(jobApplicant);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Job applicant {UniqueId} submitted successfully.", jobApplicant.UniqueId);
                return Ok(new JobApplicationResponse() { Success = true, Message = "Application submitted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while submitting the job application.");
                return StatusCode(500, new JobApplicationResponse() { Success = false, Message = "An error occurred while processing your request." });
            }

        }

        [HttpGet("downloadResumeByApplicantId/{uniqueId}")]
        public async Task<IActionResult> DownloadResume(string uniqueId)
        {
            try
            {
                _logger.LogInformation("Fetching resume for job applicant {UniqueId}.", uniqueId);
                var jobApplicant = await _repository.GetJobApplicantByUniqueIdAsync(uniqueId);

                if (jobApplicant == null)
                {
                    _logger.LogWarning("Job applicant {UniqueId} not found.", uniqueId);
                    return NotFound(new JobApplicationResponse() { Success = false, Message = "Job applicant not found." });
                }

                if (string.IsNullOrEmpty(jobApplicant.ResumeFileName))
                {
                    _logger.LogWarning("Resume file path is required for job applicant {UniqueId}.", uniqueId);
                    return BadRequest(new JobApplicationResponse() { Success = false, Message = "File path is required." });
                }

                var relativePath = $"Resumes/{jobApplicant.ResumeFileName}";
                var fileData = await _blobService.DownloadFileBlobAsync(uniqueId, relativePath);

                if (fileData == null)
                {
                    _logger.LogWarning("Resume file not found for job applicant {UniqueId}.", uniqueId);
                    return NotFound(new JobApplicationResponse() { Success = false, Message = "File not found." });
                }

                _logger.LogInformation("Resume file fetched successfully for job applicant {UniqueId}.", uniqueId);
                return File(fileData, "application/octet-stream", jobApplicant.ResumeFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading the resume for job applicant {UniqueId}.", uniqueId);
                return StatusCode(500, new JobApplicationResponse() { Success = false, Message = "An error occurred while processing your request." });
            }
        }

        [HttpGet("downloadCertificationsByApplicantId/{uniqueId}")]
        public async Task<IActionResult> DownloadCertifications(string uniqueId)
        {
            try
            {
                _logger.LogInformation("Fetching certifications for job applicant {UniqueId}.", uniqueId);
                var jobApplicant = await _repository.GetJobApplicantByUniqueIdAsync(uniqueId);

                if (jobApplicant == null)
                {
                    _logger.LogWarning("Job applicant {UniqueId} not found.", uniqueId);
                    return NotFound(new JobApplicationResponse() { Success = false, Message = "Job applicant not found." });
                }

                if (jobApplicant.CertificationsFilesNames.Count < 1)
                {
                    _logger.LogWarning("No certifications found for job applicant {UniqueId}.", uniqueId);
                    return BadRequest(new JobApplicationResponse() { Success = false, Message = "No certifications found." });
                }

                var zipName = $"{jobApplicant.Name}_Certifications.zip";
                var zipPath = Path.Combine(Path.GetTempPath(), zipName);

                using (var archive = new ZipArchive(new FileStream(zipPath, FileMode.Create), ZipArchiveMode.Create))
                {
                    foreach (var cert in jobApplicant.CertificationsFilesNames)
                    {
                        var relativePath = $"Certifications/{cert}";

                        var fileData = await _blobService.DownloadFileBlobAsync(uniqueId, relativePath);
                        if (fileData != null)
                        {
                            var entry = archive.CreateEntry(cert);
                            using (var entryStream = entry.Open())
                            {
                                await entryStream.WriteAsync(fileData, 0, fileData.Length);
                            }
                        }
                    }
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(zipPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                System.IO.File.Delete(zipPath);

                _logger.LogInformation("Certifications fetched successfully for job applicant {UniqueId}.", uniqueId);
                return File(memory, "application/zip", zipName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading certifications for job applicant {UniqueId}.", uniqueId);
                return StatusCode(500, new JobApplicationResponse() { Success = false, Message = "An error occurred while processing your request." });
            }
        }

        [HttpGet("getAllJobApplicants")]
        public async Task<IActionResult> GetAllJobApplicants()
        {
            try
            {
                _logger.LogInformation("Fetching all job applicants.");
                var jobApplicants = await _repository.GetAllJobApplicantsAsync();

                return Ok(_mapper.Map<List<JobApplicantViewModel>>(jobApplicants));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all job applicants.");
                return StatusCode(500, new JobApplicationResponse() { Success = false, Message = "An error occurred while processing your request." });
            }
        }

        [HttpGet("getJobApplicantById/{uniqueId}")]
        public async Task<IActionResult> GetJobApplicantById(string uniqueId)
        {
            try
            {
                _logger.LogInformation("Fetching job applicant by unique ID {UniqueId}.", uniqueId);
                var jobApplicant = await _repository.GetJobApplicantByUniqueIdAsync(uniqueId);

                if (jobApplicant == null)
                {
                    _logger.LogWarning("Job applicant {UniqueId} not found.", uniqueId);
                    return NotFound(new JobApplicationResponse() { Success = false, Message = "Job applicant not found." });
                }

                return Ok(_mapper.Map<JobApplicantViewModel>(jobApplicant));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching job applicant by unique ID {UniqueId}.", uniqueId);
                return StatusCode(500, new JobApplicationResponse() { Success = false, Message = "An error occurred while processing your request." });
            }
        }
    }
}
