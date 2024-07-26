namespace JobApplicationPortal.Models.Interfaces
{
    public interface IApplicant
    {
        int Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Position { get; set; }
        string ResumeFileUrl { get; set; }
        List<string> CertificationFilesUrls { get; set; }
    }
}
