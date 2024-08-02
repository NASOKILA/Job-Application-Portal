using System.Text;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using JobApplicationPortal.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace JobApplicationPortal.Tests.Services
{
    [TestFixture]
    public class BlobServiceTests
    {
        private BlobService _blobService;

        private IConfiguration _configuration;

        private BlobServiceClient _blobServiceClient;

        private BlobContainerClient _blobContainerClient;

        private BlobClient _blobClient;

        [SetUp]
        public void SetUp()
        {
            _configuration = Substitute.For<IConfiguration>();
            _configuration.GetConnectionString("AzureBlobStorage").Returns("UseDevelopmentStorage=true");

            _blobServiceClient = Substitute.For<BlobServiceClient>(_configuration.GetConnectionString("AzureBlobStorage"));
            _blobContainerClient = Substitute.For<BlobContainerClient>();
            _blobClient = Substitute.For<BlobClient>();

            _blobServiceClient.GetBlobContainerClient(Arg.Any<string>()).Returns(_blobContainerClient);
            _blobContainerClient.GetBlobClient(Arg.Any<string>()).Returns(_blobClient);
            _blobClient.CanGenerateSasUri.Returns(true);

            _blobService = new BlobService(_blobServiceClient);
        }

        [Test]
        public async Task UploadFileBlobAsync_UploadsFileAndReturnsSasUri()
        {
            // Arrange
            var file = Substitute.For<IFormFile>();
            var fileName = "JohnWickResume.pdf";
            var containerName = "johnwick";

            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("Fake file content"));
            file.OpenReadStream().Returns(fileStream);
            file.ContentType.Returns("application/pdf");

            var sasUri = new Uri("https://example.com/sasuri");
            _blobClient.GenerateSasUri(Arg.Any<BlobSasBuilder>()).Returns(sasUri);

            // Act
            var result = await _blobService.UploadFileBlobAsync(file, containerName, fileName);

            // Assert
            Assert.AreEqual(sasUri.ToString(), result);
            await _blobContainerClient.Received(1).CreateIfNotExistsAsync();
            await _blobClient.Received(1).UploadAsync(fileStream, Arg.Is<BlobHttpHeaders>(h => h.ContentType == file.ContentType));
        }

        //[Test]
        //public async Task DownloadFileBlobAsync_ReturnsFileContentIfBlobExists()
        //{
        //    // Arrange
        //    var containerName = "johnwick";
        //    var relativePath = "resumes/JohnWickResume.pdf";
        //    var fileContent = Encoding.UTF8.GetBytes("Fake file content");

        //    var memoryStream = new MemoryStream(fileContent);

        //    var blobDownloadInfo = Substitute.For<BlobDownloadInfo>(memoryStream, new BlobHttpHeaders(), null, null, null, default, null, null, null, null, null, null, null);
        //    var downloadInfo = Response.FromValue(blobDownloadInfo, Substitute.For<Response>());

        //    _blobClient.ExistsAsync().Returns(Task.FromResult(Response.FromValue(true, null)));
        //    _blobClient.DownloadAsync().Returns(Task.FromResult(downloadInfo));

        //    // Act
        //    var result = await _blobService.DownloadFileBlobAsync(containerName, relativePath);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(fileContent, result);
        //}

        [Test]
        public async Task DownloadFileBlobAsync_ReturnsNullIfBlobDoesNotExist()
        {
            // Arrange
            var containerName = "johnwick";
            var relativePath = "resumes/JohnWickResume.pdf";

            _blobClient.ExistsAsync().Returns(Task.FromResult(Response.FromValue(false, null)));

            // Act
            var result = await _blobService.DownloadFileBlobAsync(containerName, relativePath);

            // Assert
            Assert.IsNull(result);
        }
    }
}
