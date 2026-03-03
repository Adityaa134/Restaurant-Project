using System;
using Microsoft.AspNetCore.Http;

namespace Restaurent.Core.ServiceContracts
{
    public interface IImageUpdateService
    {
        /// <summary>
        /// Replace the existing image in Azure Blob Storage
        /// </summary>
        /// <param name="imageFile">the new image</param>
        /// <param name="existingUrl">the existing image url</param>
        /// <returns>Returns new image url</returns>
        Task<string> ImageUpdater(IFormFile imageFile, string existingUrl);
    }
}
