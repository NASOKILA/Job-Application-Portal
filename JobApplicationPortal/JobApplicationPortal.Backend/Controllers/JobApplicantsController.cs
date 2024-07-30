using AutoMapper;
using JobApplicationPortal.DB;
using JobApplicationPortal.Models;
using JobApplicationPortal.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace JobApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicantsController : ControllerBase
    {
        private readonly JobApplicationPortalDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public JobApplicantsController(JobApplicationPortalDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromForm] JobApplicantModel model)
        {
            if (ModelState.IsValid)
            {
                var jobApplicant = _mapper.Map<JobApplicants>(model);
                
                jobApplicant.UniqueId = Guid.NewGuid().ToString();

                if (model.Resume != null)
                {
                    var resumePath = Path.Combine(_env.ContentRootPath, "Resumes", $"{jobApplicant.UniqueId}_{model.Resume.FileName}");
                    using (var stream = new FileStream(resumePath, FileMode.Create))
                    {
                        await model.Resume.CopyToAsync(stream);
                    }
                    jobApplicant.ResumeFileName = $"{jobApplicant.UniqueId}_{model.Resume.FileName}";
                }

                if (model.Certifications != null)
                {
                    foreach (var certification in model.Certifications)
                    {
                        var certificationPath = Path.Combine(_env.ContentRootPath, "Certifications", $"{jobApplicant.UniqueId}_{certification.FileName}");
                        using (var stream = new FileStream(certificationPath, FileMode.Create))
                        {
                            await certification.CopyToAsync(stream);
                        }
                        jobApplicant.CertificationsFilesNames.Add($"{jobApplicant.UniqueId}_{certification.FileName}");
                    }
                }

                _context.JobApplicants.Add(jobApplicant);

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Application submitted successfully." });
            }

            return BadRequest(new { success = false, message = "Invalid data submitted." });
        }


        [HttpGet("downloadResumeByApplicantId/{uniqueId}")]
        public async Task<IActionResult> DownloadResume(string uniqueId)
        {
            var jobApplicant = await _context.JobApplicants.FirstOrDefaultAsync(x => x.UniqueId == uniqueId);

            if (jobApplicant == null)
            {
                return NotFound(new { success = false, message = "Job applicant not found." });
            }

            if (string.IsNullOrEmpty(jobApplicant.ResumeFileName))
            {
                return BadRequest(new { success = false, message = "File path is required." });
            }

            var fullPath = Path.Combine(_env.ContentRootPath, "Resumes/" + jobApplicant.ResumeFileName);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(new { success = false, message = "File not found." });
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;

            var file = Path.GetFileName(fullPath).Split("_")[1];

            return File(memory, "APPLICATION/octet-stream", file);
        }


        [HttpGet("downloadCertificationsByApplicantId/{uniqueId}")]
        public async Task<IActionResult> DownloadCertifications(string uniqueId)
        {
            var jobApplicant = await _context.JobApplicants.FirstOrDefaultAsync(x => x.UniqueId == uniqueId);

            if (jobApplicant == null)
            {
                return NotFound(new { success = false, message = "Job applicant not found." });
            }

            if (jobApplicant.CertificationsFilesNames.Count < 1)
            {
                return BadRequest(new { success = false, message = "No certifications found." });
            }

            var zipName = $"{jobApplicant.Name}_Certifications.zip";
            var zipPath = Path.Combine(_env.ContentRootPath, "Certifications", zipName);

            using (var archive = new ZipArchive(new FileStream(zipPath, FileMode.Create), ZipArchiveMode.Create))
            {
                foreach (var cert in jobApplicant.CertificationsFilesNames)
                {
                    var fullPath = Path.Combine(_env.ContentRootPath, "Certifications", cert);
                    if (System.IO.File.Exists(fullPath))
                    {
                        archive.CreateEntryFromFile(fullPath, Path.GetFileName(fullPath).Split("_")[1]);
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
            var jobApplicants = await _context.JobApplicants.ToListAsync();

            var mappedJobApplicants = _mapper.Map<List<JobApplicants>>(jobApplicants);

            return Ok(mappedJobApplicants);
        }


        [HttpGet("getJobApplicantById/{uniqueId}")]
        public async Task<IActionResult> GetJobApplicantById(string uniqueId)
        {
            var jobApplicant = await _context.JobApplicants.FirstOrDefaultAsync(x => x.UniqueId == uniqueId);

            if (jobApplicant == null)
            {
                return NotFound(new { success = false, message = "Job applicant not found." });
            }

            return Ok(jobApplicant);
        }
    }
}
