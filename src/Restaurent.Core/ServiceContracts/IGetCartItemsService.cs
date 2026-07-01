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
    }
}
