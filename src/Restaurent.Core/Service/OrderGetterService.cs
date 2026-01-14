using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class OrderGetterService : IOrderGetterService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IAuthService _authService;
        public OrderGetterService(IOrdersRepository ordersRepository, IAuthService authService)
        {
            _ordersRepository = ordersRepository;
            _authService = authService;
        }

        public async Task<OrderResponse?> GetOrderByOrderId(Guid orderId)
        {
            if(orderId==Guid.Empty)
                throw new ArgumentNullException(nameof(orderId));
            OrderResponse? order =  await _ordersRepository.GetOrderByOrderId(orderId);
            if (order == null)
                return null;
            return order;
        }

        public async Task<PaginationResponse<OrderResponse>?> GetOrders(PaginationRequest request)
        {
            PaginationResponse<OrderResponse>? orders =  await _ordersRepository.GetOrders(request.Page,request.PageSize);
            return orders;
        }

        public async Task<List<OrderResponse>?> GetOrdersByUserId(Guid userId)
        {
            if(userId==Guid.Empty)
                throw new ArgumentNullException(nameof(userId));
            UserDTO? userDTO =  await _authService.GetUserByUserId(userId);
            if(userDTO == null)
                throw new ArgumentException("Invalid User Id");
            List<OrderResponse>? orders =  await _ordersRepository.GetOrdersByUserId(userId);
            return orders;
        }
    }
}
