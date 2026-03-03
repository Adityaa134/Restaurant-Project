using System;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class ImageUpdateService : IImageUpdateService
    {
        private readonly IConfiguration _configuration;
        private readonly IImageDeleteService _imageDeleteService;

        public ImageUpdateService(IConfiguration configuration, IImageDeleteService imageDeleteService)
        {
            _configuration = configuration;
            _imageDeleteService = imageDeleteService;
        }

        public async Task<string> ImageUpdater(IFormFile imageFile, string existingUrl)
        {
            if (string.IsNullOrWhiteSpace(existingUrl))
                throw new ArgumentException("Existing image URL cannot be null or empty");

            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("New image file is required");

            string containerName = _configuration["BlobStorage:ContainerName"]!;
            string connectionString = _configuration["BlobStorage:ConnectionString"]!;

            BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            string extension = Path.GetExtension(imageFile.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";

            BlobClient blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

            using var stream = imageFile.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            await _imageDeleteService.ImageDeleter(existingUrl); 

            return blobClient.Uri.AbsoluteUri;
        }
    }
}
