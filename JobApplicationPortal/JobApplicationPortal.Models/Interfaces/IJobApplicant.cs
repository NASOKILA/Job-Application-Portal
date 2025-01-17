﻿namespace JobApplicationPortal.Models.Interfaces
{
    public interface IJobApplicant
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Position { get; set; }
        string ResumeFileName { get; set; }
        List<string> CertificationsFilesNames { get; set; }
    }
}
