using System;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class ImageDeleteService : IImageDeleteService
    {
        private readonly IConfiguration _configuration;

        public ImageDeleteService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

       
         public async Task<bool> ImageDeleter(string imagePath)
         {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            string containerName = _configuration["BlobStorage:ContainerName"]!;
            string connectionString = _configuration["BlobStorage:ConnectionString"]!;


            BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, containerName);
            Uri oldUri = new Uri(imagePath);
            string oldFileName = Path.GetFileName(oldUri.LocalPath);
            BlobClient blobClient = blobContainerClient.GetBlobClient(oldFileName);

            return await blobClient.DeleteIfExistsAsync();
         }
    }
}