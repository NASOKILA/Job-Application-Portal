using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using JobApplicationPortal.Models.Interfaces;

namespace JobApplicationPortal.Models.DbModels
{
    [ExcludeFromCodeCoverage]
    [Table("Applicant")]
    public class Applicant : IApplicant, IEntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public string Position { get; set; }

        [Required(ErrorMessage = "ResumeFileUrl is required")]
        public string ResumeFileUrl { get; set; }

        public List<string> CertificationFilesUrls { get; set; }
    }
}
