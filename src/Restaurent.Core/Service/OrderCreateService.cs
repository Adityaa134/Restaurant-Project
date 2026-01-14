using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Helpers;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class OrderCreateService : IOrderCreateService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IDishGetterService _dishGetterService;
        private readonly IAuthService _authService;
        private readonly IAddressService _addressService;
        private readonly IRemoveCartItemsService _removeCartItemsService;
        public OrderCreateService(IOrdersRepository ordersRepository, IDishGetterService dishGetterService, IAuthService authService, IAddressService addressService, IRemoveCartItemsService removeCartItemsService)
        {
            _ordersRepository = ordersRepository;
            _dishGetterService = dishGetterService;
            _authService = authService;
            _addressService = addressService;
            _removeCartItemsService = removeCartItemsService;
        }
        public async Task<OrderResponse> CreateOrder(OrderAddRequest orderAddRequest)
        {
            if (orderAddRequest == null)
                throw new ArgumentException(nameof(orderAddRequest));

            ValidationHelper.ModelValidator(orderAddRequest);
            decimal totalBill = 0;

            foreach(OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
            {
                ValidationHelper.ModelValidator(orderItemAddRequest);
                DishResponse? dishResponse =  await _dishGetterService.GetDishByDishId(orderItemAddRequest.DishId);
                if (dishResponse == null)
                    throw new ArgumentException("Invalid Dish Id");
            }
            
            UserDTO? userDTO =   await _authService.GetUserByUserId(orderAddRequest.UserId.Value);
            if (userDTO == null)
                throw new ArgumentException("Invalid User Id");
            AddressResponse addressResponse = await _addressService.GetAddressByAddressId(orderAddRequest.DeliveryAddressId);
            if (addressResponse == null)
                throw new ArgumentException("Invalid address id ");

            Order order = orderAddRequest.ToOrder();
            order.Id = Guid.NewGuid();

            foreach (OrderItem orderItem in order.OrderItems)
            {
                orderItem.Id = Guid.NewGuid();
                totalBill += orderItem.Quantity * orderItem.UnitPrice;
            }
            order.TotalBill = totalBill;

            await _ordersRepository.CreateOrder(order);
            await _removeCartItemsService.RemoveItemsFromCartByUserId(order.UserId);
            return order.ToOrderResponse();
        }
    }
}
