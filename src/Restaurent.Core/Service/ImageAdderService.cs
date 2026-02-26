using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class ImageAdderService : IImageAdderService
    {
        private readonly IHostEnvironment _hostEnvironment;

        public ImageAdderService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<string> ImageAdder(IFormFile imageFile, string subFolder="Images")
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentNullException(nameof(imageFile), "Image file cannot be null or empty");
            }

            var wwwrootPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot");

            
            var targetFolder = Path.Combine(wwwrootPath, subFolder);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

           
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(targetFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }


            return $"/{subFolder}/{uniqueFileName}";
        }
    } 
}

