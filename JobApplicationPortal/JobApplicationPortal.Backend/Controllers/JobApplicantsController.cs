using AutoMapper;
using JobApplicationPortal.DB;
using JobApplicationPortal.Models;
using JobApplicationPortal.Models.DbModels;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> SubmitJobApplication([FromForm] JobApplicantModel model)
        {
            if (ModelState.IsValid)
            {
                var jobApplicant = _mapper.Map<JobApplicant>(model); 
                
                if (model.Resume != null)
                {
                    var resumePath = Path.Combine(_env.ContentRootPath, "Uploads", model.Resume.FileName);
                    using (var stream = new FileStream(resumePath, FileMode.Create))
                    {
                        await model.Resume.CopyToAsync(stream);
                    }
                    jobApplicant.ResumeFilePath = resumePath;
                }

                if (model.Certifications != null)
                {
                    foreach (var certification in model.Certifications)
                    {
                        var certificationPath = Path.Combine(_env.ContentRootPath, "Uploads", certification.FileName);
                        using (var stream = new FileStream(certificationPath, FileMode.Create))
                        {
                            await certification.CopyToAsync(stream);
                        }
                        jobApplicant.CertificationsFilesPath.Add(certificationPath);
                    }
                }

                _context.JobApplicants.Add(jobApplicant);

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Application submitted successfully." });
            }

            return BadRequest(new { success = false, message = "Invalid data submitted." });
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return BadRequest(new { success = false, message = "File path is required." });
            }

            var fullPath = Path.Combine(_env.ContentRootPath, filePath);

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

            var fileName = Path.GetFileName(fullPath);

            return File(memory, "APPLICATION/octet-stream", fileName);
        }
    }
}
