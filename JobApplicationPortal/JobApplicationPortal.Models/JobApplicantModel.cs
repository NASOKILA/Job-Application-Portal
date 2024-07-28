using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace JobApplicationPortal.Models
{
    // Model for receiving form data with files
    public class JobApplicantModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Resume is required")]
        public IFormFile Resume { get; set; }
        public List<IFormFile> Certifications { get; set; }
    }
}
