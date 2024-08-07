using AutoMapper;
using FluentValidation;
using JobApplicationPortal.Backend.Responses;
using JobApplicationPortal.Backend.Services;
using JobApplicationPortal.DB;
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
        private readonly JobApplicantService _jobApplicantService;
        private readonly IBlobService _blobService;
        private readonly IMapper _mapper;
        private readonly IValidator<JobApplicantDto> _validator;

        public JobApplicantsController(JobApplicantService jobApplicantService, JobApplicationPortalDbContext context, IBlobService blobService, IMapper mapper, IValidator<JobApplicantDto> validator, ILogger<JobApplicantsController> logger)
        {
            _jobApplicantService = jobApplicantService;
            _validator = validator;
            _blobService = blobService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromForm] JobApplicantDto model)
        {
            var result = await _validator.ValidateAsync(model);
            
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => new ValidationError
                {
                    PropertyName = e.PropertyName,
                    ErrorMessage = e.ErrorMessage
                }).ToList();

                return BadRequest(new JobApplicationResponse() { Success = false, Message = "Validation errors occurred.", Errors = errors });
            }

            var jobApplicant = await _jobApplicantService.AddJobApplicantAsync(model);

            return Ok(new JobApplicationResponse() { Success = true, Message = "Application submitted successfully." });
        }

        [HttpGet("{id:guid}/resume")]
        public async Task<IActionResult> DownloadResume(Guid id)
        {
            var jobApplicant = await _jobApplicantService.GetJobApplicantByIdAsync(id);

            if (jobApplicant == null || string.IsNullOrEmpty(jobApplicant.ResumeFileName))
            {
                return NotFound(new JobApplicationResponse() { Success = false, Message = "Job applicant not found." });
            }

            var relativePath = $"Resumes/{jobApplicant.ResumeFileName}";

            var fileData = await _blobService.DownloadFileBlobAsync(id.ToString(), relativePath);

            if (fileData == null)
            {
                return NotFound(new JobApplicationResponse() { Success = false, Message = "File not found." });
            }

            return File(fileData, "application/octet-stream", jobApplicant.ResumeFileName);
        }

        [HttpGet("{id:guid}/certifications")]
        public async Task<IActionResult> DownloadCertifications(Guid id)
        {
            var jobApplicant = await _jobApplicantService.GetJobApplicantByIdAsync(id);
            if (jobApplicant == null || jobApplicant.CertificationsFilesNames.Count < 1)
            {
                return NotFound(new JobApplicationResponse { Success = false, Message = "Job applicant or certifications not found." });
            }

            var zipName = $"{jobApplicant.Name}_Certifications.zip";
            var zipPath = Path.Combine(Path.GetTempPath(), zipName);

            using (var archive = new ZipArchive(new FileStream(zipPath, FileMode.Create), ZipArchiveMode.Create))
            {
                foreach (var cert in jobApplicant.CertificationsFilesNames)
                {
                    var relativePath = $"Certifications/{cert}";
                    var fileData = await _blobService.DownloadFileBlobAsync(id.ToString(), relativePath);
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

            return File(memory, "application/zip", zipName);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobApplicants()
        {
            var jobApplicants = await _jobApplicantService.GetAllJobApplicantsAsync();
            return Ok(_mapper.Map<List<JobApplicantViewModel>>(jobApplicants));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetJobApplicantById(Guid id)
        {
            var jobApplicant = await _jobApplicantService.GetJobApplicantByIdAsync(id);
            if (jobApplicant == null)
            {
                return NotFound(new JobApplicationResponse { Success = false, Message = "Job applicant not found." });
            }

            return Ok(_mapper.Map<JobApplicantViewModel>(jobApplicant));
        }
    }
}
