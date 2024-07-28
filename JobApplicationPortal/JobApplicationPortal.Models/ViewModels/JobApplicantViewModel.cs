using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.Models.ViewModels
{
    public class JobApplicantViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string ResumeFilePath { get; set; }
        public List<string> CertificationsFilesPath { get; set; }
    }
}
