using System;
using Microsoft.Extensions.Hosting;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class ImageDeleteService : IImageDeleteService
    {
        private readonly IHostEnvironment _hostEnvironment;

        public ImageDeleteService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

       
         public async Task<bool> ImageDeleter(string imagePath)
         {
            if (string.IsNullOrEmpty(imagePath))
                return false;


          
            var cleanPath = imagePath.TrimStart('/', '\\');

            
            var fullPath = Path.Combine(
                _hostEnvironment.ContentRootPath,
                "wwwroot",
                cleanPath);

            
            fullPath = fullPath.Replace('/', Path.DirectorySeparatorChar)
                              .Replace('\\', Path.DirectorySeparatorChar);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
         }
    }
}