using Restaurent.Core.Domain.Entities;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IOrderGetterService
    {
        /// <summary>
        /// Returns orders form the data store
        /// </summary>
        /// <param name="request">Page details for orders</param>
        /// <returns>Returns orders form the data store</returns>
        Task<PaginationResponse<OrderResponse>?> GetOrders(PaginationRequest request);

        /// <summary>
        /// Search for the order based on the order id 
        /// </summary>
        /// <param name="orderId">the order id to search</param>
        /// <returns>Returns the order from the data store</returns>
        Task<OrderResponse?> GetOrderByOrderId(Guid orderId);

        /// <summary>
        /// Returns the orders of the user 
        /// </summary>
        /// <param name="userId">The user orders to search</param>
        /// <returns>Returns the orders of the user from the data store</returns>
        Task<List<OrderResponse>?> GetOrdersByUserId(Guid userId);

        /// <summary>
        /// Checks whether the given dish exists in the given order
        /// </summary>
        /// <param name="orderId">The order to check</param>
        /// <param name="dishId">The dish to check</param>
        /// <returns>Returns true if the dish is part of the order otherwise false</returns>
        Task<bool> IsDishPartOfOrder(Guid orderId, Guid dishId);

        /// <summary>
        /// Checks whether the given order belongs to the given user
        /// </summary>
        /// <param name="userId">The user to check</param>
        /// <param name="orderId">The order to check</param>
        /// <returns>Returns true if the order belongs to the user otherwise false</returns>
        Task<bool> IsOrderOwnedByUser(Guid userId, Guid orderId);
    }
}
