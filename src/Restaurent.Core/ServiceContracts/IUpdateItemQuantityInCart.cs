using System;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IUpdateItemQuantityInCart
    {
        /// <summary>
        /// Updates the dish quantity in cart of a specific user
        /// </summary>
        /// <param name="updateQuantityRequest">Quantity to update</param>
        /// <returns>Returns The updated cart details</returns>
        Task<CartResponse> UpdateDishQuantityInCartItem(UpdateQuantityRequest updateQuantityRequest);
    }
}
