using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using JobApplicationPortal.Models.Interfaces;

namespace JobApplicationPortal.Models.DbModels
{
    [ExcludeFromCodeCoverage]
    [Table("JobApplicants")]
    public class JobApplicants : IJobApplicant, IEntityBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Position is required")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Resume File Name is required")]
        public string ResumeFileName { get; set; }

        public List<string> CertificationsFilesNames { get; set; }
    }
}
