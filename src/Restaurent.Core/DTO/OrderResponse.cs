using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to return the order details
    /// </summary>
    public class OrderResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public string? UserName { get; set; }
        public List<OrderItemResponse>? OrderItems { get; set; }
        public decimal TotalBill { get; set; }
        public AddressResponse? DeliveryAddress { get; set; }
    }

    public static class OrderExtension
    {
        public static OrderResponse ToOrderResponse(this Order order)
        {
            return new OrderResponse()
            {
                OrderId = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                OrderStatus = order.Status.ToString(),
                TotalBill = order.TotalBill,
                OrderItems = order?.OrderItems
                                  .Select(item => item.ToOrderItemResponse())
                                  .ToList(),
                UserName = order?.User?.UserName,
                DeliveryAddress = new AddressResponse()
                {
                    AddressId = order.DeliveryAddressId,
                    UserId=order?.DeliveryAddress?.UserId,
                    AddressLine = order?.DeliveryAddress?.AddressLine,
                    Area = order?.DeliveryAddress?.Area,
                    Landmark = order?.DeliveryAddress?.Landmark,
                    City = order?.DeliveryAddress?.City,
                }
            };
        }
    }
}
