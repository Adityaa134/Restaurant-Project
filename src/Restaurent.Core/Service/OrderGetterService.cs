using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class OrderGetterService : IOrderGetterService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IAuthService _authService;
        private readonly IRatingsService _ratingsService;
        public OrderGetterService(IOrdersRepository ordersRepository, IAuthService authService, IRatingsService ratingsService)
        {
            _ordersRepository = ordersRepository;
            _authService = authService;
            _ratingsService = ratingsService;
        }

        public async Task<OrderResponse?> GetOrderByOrderId(Guid orderId)
        {
            if(orderId==Guid.Empty)
                throw new ArgumentNullException(nameof(orderId));
            OrderResponse? order =  await _ordersRepository.GetOrderByOrderId(orderId);
            if (order == null)
                return null;
            List<RatingResponse>? ratings = await _ratingsService.GetRatingsByOrderId(orderId);

            foreach (var item in order.OrderItems)
            {
                var matchingRating =
                    ratings?.FirstOrDefault(r => r.DishId == item.DishId);

                if (matchingRating != null)
                {
                    item.Rating = matchingRating.Rate;
                    item.Comment = matchingRating.Comment;
                }
            }

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

        public async Task<bool> IsDishPartOfOrder(Guid orderId, Guid dishId)
        {
            return await _ordersRepository.IsDishPartOfOrder(orderId, dishId);
        }

        public async Task<bool> IsOrderOwnedByUser(Guid userId, Guid orderId)
        {
            return await _ordersRepository.IsOrderOwnedByUser(userId, orderId);
        }
    }
}
