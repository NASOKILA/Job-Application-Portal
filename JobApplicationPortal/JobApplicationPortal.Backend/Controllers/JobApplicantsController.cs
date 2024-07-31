using AutoMapper;
using JobApplicationPortal.Models;
using JobApplicationPortal.Models.DbModels;
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

        public JobApplicantsController(IJobApplicantsRepository repository, IBlobService blobService, IMapper mapper)
        {
            _repository = repository;
            _blobService = blobService;
            _mapper = mapper;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromForm] JobApplicantModel model)
        {
            if (ModelState.IsValid)
            {
                var jobApplicant = _mapper.Map<JobApplicants>(model);

                jobApplicant.UniqueId = Guid.NewGuid().ToString();
                var containerName = jobApplicant.UniqueId;
                if (model.Resume != null)
                {
                    var resumeFileName = $"Resumes/{model.Resume.FileName}";
                    await _blobService.UploadFileBlobAsync(model.Resume, containerName, resumeFileName);
                    jobApplicant.ResumeFileName = model.Resume.FileName;
                }

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

                return Ok(new { success = true, message = "Application submitted successfully." });
            }

            return BadRequest(new { success = false, message = "Invalid data submitted." });
        }

        [HttpGet("downloadResumeByApplicantId/{uniqueId}")]
        public async Task<IActionResult> DownloadResume(string uniqueId)
        {
            var jobApplicant = await _repository.GetJobApplicantByUniqueIdAsync(uniqueId);

            if (jobApplicant == null)
            {
                return NotFound(new { success = false, message = "Job applicant not found." });
            }

            if (string.IsNullOrEmpty(jobApplicant.ResumeFileName))
            {
                return BadRequest(new { success = false, message = "File path is required." });
            }

            var relativePath = $"Resumes/{jobApplicant.ResumeFileName}";
            var fileData = await _blobService.DownloadFileBlobAsync(uniqueId, relativePath);

            if (fileData == null)
            {
                return NotFound(new { success = false, message = "File not found." });
            }

            return File(fileData, "application/octet-stream", jobApplicant.ResumeFileName);
        }

        [HttpGet("downloadCertificationsByApplicantId/{uniqueId}")]
        public async Task<IActionResult> DownloadCertifications(string uniqueId)
        {
            var jobApplicant = await _repository.GetJobApplicantByUniqueIdAsync(uniqueId);

            if (jobApplicant == null)
            {
                return NotFound(new { success = false, message = "Job applicant not found." });
            }

            if (jobApplicant.CertificationsFilesNames.Count < 1)
            {
                return BadRequest(new { success = false, message = "No certifications found." });
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

            return File(memory, "application/zip", zipName);
        }

        [HttpGet("getAllJobApplicants")]
        public async Task<IActionResult> GetAllJobApplicants()
        {
            var jobApplicants = await _repository.GetAllJobApplicantsAsync();

            return Ok(_mapper.Map<List<JobApplicantViewModel>>(jobApplicants));
        }

        [HttpGet("getJobApplicantById/{uniqueId}")]
        public async Task<IActionResult> GetJobApplicantById(string uniqueId)
        {
            var jobApplicant = await _repository.GetJobApplicantByUniqueIdAsync(uniqueId);

            if (jobApplicant == null)
            {
                return NotFound(new { success = false, message = "Job applicant not found." });
            }

            return Ok(_mapper.Map<JobApplicantViewModel>(jobApplicant));
        }
    }
}
