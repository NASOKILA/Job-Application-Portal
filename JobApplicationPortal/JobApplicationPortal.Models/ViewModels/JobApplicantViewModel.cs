
namespace JobApplicationPortal.Models.ViewModels
{
    public class JobApplicantViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string ResumeFileName{ get; set; }
        public List<string> CertificationsFilesNames { get; set; }
    }
}
