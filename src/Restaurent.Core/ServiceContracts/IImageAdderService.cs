using System;
using Microsoft.AspNetCore.Http;

namespace Restaurent.Core.ServiceContracts
{
    public interface IImageAdderService
    {
        /// <summary>
        /// Uploads image to Azure Blob Storage
        /// </summary>
        /// <param name="imageFile">the image to add</param>
        /// <returns>Returns the path of image </returns>
         Task<string> ImageAdder(IFormFile imageFile);
    }
}
