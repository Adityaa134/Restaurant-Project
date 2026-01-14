using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Enums;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class OrderUpdateService : IOrderUpdateService
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrderUpdateService(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<OrderResponse?> UpdateOrderStatusToCancel(UpdateOrderStatusRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Order? order = await _ordersRepository.GetOrderDetailsByOrderId(request.OrderId);
            if (order == null)
                throw new ArgumentException("Invalid Order Id");
            if (request.OrderStatus != OrderStatus.Cancelled)
                throw new InvalidOperationException("Only cancellation can be done");

            if(order.Status == OrderStatus.Pending || order.Status == OrderStatus.Preparing)
            {
                OrderResponse? updatedOrderResponse = await _ordersRepository.UpdateOrderStatus(request.OrderStatus, request.OrderId);
                return updatedOrderResponse;
            }
            throw new InvalidOperationException("Order status can't be change");
            
        }

        public async Task<OrderResponse?> UpdateOrderStatus(UpdateOrderStatusRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Order? order = await _ordersRepository.GetOrderDetailsByOrderId(request.OrderId);
            if (order == null)
                throw new ArgumentException("Invalid Order Id");

            if (!IsValidTransition(order.Status, request.OrderStatus))
                throw new InvalidOperationException(
                    $"Cannot change order status from {order.Status} to {request.OrderStatus}"
                );

            OrderResponse? updatedOrderResponse = await _ordersRepository.UpdateOrderStatus(request.OrderStatus, request.OrderId);
                return updatedOrderResponse;

        }

        private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions =
        new()
        {
            { OrderStatus.Pending,    new[] { OrderStatus.Preparing, OrderStatus.Confirmed, OrderStatus.Delivered } },
            { OrderStatus.Preparing,  new[] { OrderStatus.Confirmed, OrderStatus.Delivered} },
            { OrderStatus.Confirmed,  new[] { OrderStatus.Delivered } },
            { OrderStatus.Delivered,  Array.Empty<OrderStatus>() },
            { OrderStatus.Cancelled,  Array.Empty<OrderStatus>() }
        };

        private static bool IsValidTransition(OrderStatus current, OrderStatus next)
        {
            return AllowedTransitions.TryGetValue(current, out var allowed)
                   && allowed.Contains(next);
        }
    }
}
