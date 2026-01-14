using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Enums;
using Restaurent.Infrastructure.DBContext;

namespace Restaurent.Infrastructure.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public OrdersRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }
        
        public async Task<OrderResponse?> GetOrderByOrderId(Guid orderId)
        {
            return await _dbContext.Orders
                .Where(o => o.Id == orderId)
                .Select(o => new OrderResponse
                {
                    OrderId = o.Id,
                    UserId = o.UserId,
                    UserName = o.User.UserName,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.Status.ToString(),
                    TotalBill = o.TotalBill,

                    DeliveryAddress = new AddressResponse
                    {
                        AddressId = o.DeliveryAddressId,
                        UserId = o.DeliveryAddress.UserId,
                        AddressLine = o.DeliveryAddress.AddressLine,
                        Area = o.DeliveryAddress.Area,
                        City = o.DeliveryAddress.City,
                        Landmark = o.DeliveryAddress.Landmark
                    },

                    OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                    {
                        DishId = oi.DishId,
                        DishName = oi.Dish.DishName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<Order?> GetOrderDetailsByOrderId(Guid orderId)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(o=>o.Id == orderId);
        }

        public async Task<PaginationResponse<OrderResponse>?> GetOrders(int page,int pageSize)
        {
            var totalOrders = await _dbContext.Orders.CountAsync();
            var orders =  await _dbContext.Orders
                 .OrderByDescending(o => o.OrderDate)
                 .Skip((page-1)*pageSize)
                 .Take(pageSize)
                 .Select(o => new OrderResponse
                 {
                     OrderId = o.Id,
                     UserId = o.UserId,
                     UserName = o.User.UserName,
                     OrderDate = o.OrderDate,
                     OrderStatus = o.Status.ToString(),
                     TotalBill = o.TotalBill,

                     DeliveryAddress = new AddressResponse
                     {
                         AddressId = o.DeliveryAddressId,
                         AddressLine = o.DeliveryAddress.AddressLine,
                         UserId = o.DeliveryAddress.UserId,
                         Area = o.DeliveryAddress.Area,
                         City = o.DeliveryAddress.City,
                         Landmark = o.DeliveryAddress.Landmark
                     },

                     OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                     {
                         DishId = oi.DishId,
                         DishName = oi.Dish.DishName,
                         Quantity = oi.Quantity,
                         UnitPrice = oi.UnitPrice,
                         TotalPrice = oi.Quantity * oi.UnitPrice
                     }).ToList()
                 })
                 .AsNoTracking()
                 .ToListAsync();

            return new PaginationResponse<OrderResponse>()
            {
                TotalCount = totalOrders,
                Items = orders,
                PageSize = pageSize,
                CurrentPage = page
            };
        }

        public async Task<List<OrderResponse>?> GetOrdersByUserId(Guid userId)
        {
            return await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderResponse
                {
                    OrderId = o.Id,
                    UserId = o.UserId,
                    UserName = o.User.UserName,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.Status.ToString(),
                    TotalBill = o.TotalBill,

                    DeliveryAddress = new AddressResponse
                    {
                        AddressId = o.DeliveryAddressId,
                        AddressLine = o.DeliveryAddress.AddressLine,
                        UserId = o.DeliveryAddress.UserId,
                        Area = o.DeliveryAddress.Area,
                        City = o.DeliveryAddress.City,
                        Landmark = o.DeliveryAddress.Landmark
                    },

                    OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                    {
                        DishId = oi.DishId,
                        DishName = oi.Dish.DishName,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> OrderExists(Guid orderId)
        {
            return await _dbContext.Orders.AnyAsync(o => o.Id == orderId);
        }

        public async Task<OrderResponse?> UpdateOrderStatus(OrderStatus orderStatus, Guid orderId)
        {
            Order? order =  await GetOrderDetailsByOrderId(orderId);
            if (order == null)
                return null;
            order.Status = orderStatus;
            await _dbContext.SaveChangesAsync();
            return await GetOrderByOrderId(orderId);
        }
    }
}
