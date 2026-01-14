using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IOrderUpdateService
    {
        /// <summary>
        /// Update the order status
        /// </summary>
        /// <param name="request">The status deatils to update the order</param>
        /// <returns>Returns the updated details of order</returns>
        Task<OrderResponse?> UpdateOrderStatus(UpdateOrderStatusRequest request);

        /// <summary>
        /// The order to cancel
        /// </summary>
        /// <param name="request">The status deatils to cancel the order</param>
        /// <returns>Returns the updated details of order</returns>
        Task<OrderResponse?> UpdateOrderStatusToCancel(UpdateOrderStatusRequest request);
    }
}