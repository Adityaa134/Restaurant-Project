using System;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class ImageAdderService : IImageAdderService
    {
        private readonly IConfiguration _configuration;

        public ImageAdderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> ImageAdder(IFormFile imageFile)
        {
            if(imageFile==null || imageFile.Length ==0)
                throw new ArgumentNullException("Image file cannot be null or empty");

            string containerName = _configuration["BlobStorage:ContainerName"]!;
            string connectionString = _configuration["BlobStorage:ConnectionString"]!;


            BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, containerName);
            await blobContainerClient.CreateIfNotExistsAsync();
            await blobContainerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            string extension = Path.GetExtension(imageFile.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";

            BlobClient blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

            using var stream = imageFile.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.AbsoluteUri;
        }
    } 
}

