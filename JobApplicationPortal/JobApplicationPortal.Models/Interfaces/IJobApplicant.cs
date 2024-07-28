namespace JobApplicationPortal.Models.Interfaces
{
    public interface IJobApplicant
    {
        int Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Position { get; set; }
        string ResumeFilePath { get; set; }
        List<string> CertificationsFilesPath { get; set; }
    }
}
