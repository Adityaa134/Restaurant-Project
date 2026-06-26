using System;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IAddCartItemsService
    {
        /// <summary>
        /// Adds the item in cart 
        /// </summary>
        /// <param name="addToCart">The dish to add</param>
        /// <returns></returns>
        Task<CartResponse> AddItemToCart(AddToCartRequest addToCart);

        /// <summary>
        /// Adds multiple order items to the cart during reorder operation
        /// </summary>
        /// <param name="items">The order items to add into cart</param>
        /// <returns>Returns the updated cart items</returns>
        Task<List<CartResponse>> AddOrderItemsToCart(List<ReorderCartItemRequest> items);
    }
}
