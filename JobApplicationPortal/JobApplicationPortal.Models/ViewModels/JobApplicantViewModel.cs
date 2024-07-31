
namespace JobApplicationPortal.Models.ViewModels
{
    public class JobApplicantViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string ResumeFileName{ get; set; }
        public string UniqueId { get; set; }
        public List<string> CertificationsFilesNames { get; set; }
    }
}
