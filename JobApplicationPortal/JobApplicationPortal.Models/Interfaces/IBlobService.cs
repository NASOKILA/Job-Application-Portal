using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.Models.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadFileBlobAsync(IFormFile file, string containerName, string fileName);
        Task<byte[]> DownloadFileBlobAsync(string containerName, string relativePath);
    }
}
