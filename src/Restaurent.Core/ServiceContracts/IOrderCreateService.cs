using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IOrderCreateService
    {
        /// <summary>
        /// Creates order in the data store
        /// </summary>
        /// <param name="orderAddRequest">The order to create</param>
        /// <returns>Returns the created order</returns>
        Task<OrderResponse> CreateOrder(OrderAddRequest orderAddRequest);
    }
}
