using System;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IGetCartItemsService
    {
        /// <summary>
        /// Gives all the cart items of the specific user based on user  
        /// </summary>
        /// <param name="userId">The user which cart items will be returns </param>
        /// <returns>Returns all the cart items of the specific user based on user </returns>
        Task<List<CartResponse>> GetAllCartItems(Guid? userId);


        /// <summary>
        /// Checks whether the product exist in the specific user cart or not 
        /// </summary>
        /// <param name="userId">The user which cart will be check</param>
        /// <param name="dishId">The dish to check</param>
        /// <returns>Returns true if the the specific dish is present in the current logged in user; otherwise false</returns>
        Task<bool> IsCartItemExist(Guid? userId, Guid dishId);
    }
}
