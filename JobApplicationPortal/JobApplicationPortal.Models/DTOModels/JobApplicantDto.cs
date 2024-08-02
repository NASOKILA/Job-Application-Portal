using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.Models.DTOModels
{
    public class JobApplicantDto
    {
        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public string Position { get; set; }

        public IFormFile Resume { get; set; }

        public List<IFormFile>? Certifications { get; set; }
    }
}
