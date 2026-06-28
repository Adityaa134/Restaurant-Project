using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Enums;
using Restaurent.Core.Service;
using Restaurent.Core.ServiceContracts;

namespace RestaurentSolution.UnitTests
{
    public class OrderServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IOrdersRepository> _ordersRepositoryMock;
        private readonly Mock<IRemoveCartItemsService> _removeCartItemsServiceMock;
        private readonly Mock<IDishGetterService> _dishGetterServiceMock;
        private readonly Mock<IAddressService> _addressServiceMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IRatingsService> _ratingsServiceMock;
        private readonly Mock<IAddCartItemsService> _addCartItemsServiceMock;

        private readonly IOrderCreateService _orderCreateService;
        private readonly IOrderUpdateService _orderUpdateService;
        private readonly IOrderGetterService _orderGetterService;
        public OrderServiceTests()
        {
            _fixture = new Fixture();
            _ordersRepositoryMock = new Mock<IOrdersRepository>();
            _dishGetterServiceMock = new Mock<IDishGetterService>();
            _addressServiceMock = new Mock<IAddressService>();
            _removeCartItemsServiceMock = new Mock<IRemoveCartItemsService>();
            _authServiceMock = new Mock<IAuthService>();
            _ratingsServiceMock = new Mock<IRatingsService>();
            _addCartItemsServiceMock = new Mock<IAddCartItemsService>(); 


            _orderCreateService = new OrderCreateService(_ordersRepositoryMock.Object, _dishGetterServiceMock.Object, _authServiceMock.Object, _addressServiceMock.Object, _removeCartItemsServiceMock.Object);
            _orderGetterService = new OrderGetterService(_ordersRepositoryMock.Object, _authServiceMock.Object,_ratingsServiceMock.Object,_addCartItemsServiceMock.Object);
            _orderUpdateService = new OrderUpdateService(_ordersRepositoryMock.Object);
        }


        #region GetOrder

        [Fact]
        public async Task GetOrderByOrderId_InvalidOrderId_ShouldBeNull()
        {
            OrderResponse? orderResponse = await _orderGetterService.GetOrderByOrderId(Guid.NewGuid());

            _ordersRepositoryMock.Setup(temp=>temp.GetOrderByOrderId(It.IsAny<Guid>()))
                .ReturnsAsync((OrderResponse?)null);

            orderResponse.Should().BeNull();
        }

        [Fact]
        public async Task GetOrderByOrderId_ValidOrderId_ShouldReturnMatchingOrder()
        {
             OrderResponse orderResponseExpected =
                    _fixture.Build<OrderResponse>().Create();

             List<RatingResponse> ratingsExpected =
                    orderResponseExpected.OrderItems!
                    .Select(item => new RatingResponse
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderResponseExpected.OrderId,
                        DishId = item.DishId,
                        Rate = _fixture.Create<decimal>(),
                        Comment = _fixture.Create<string>()
                    })
                    .ToList();

              _ordersRepositoryMock
                    .Setup(temp => temp.GetOrderByOrderId(It.IsAny<Guid>()))
                    .ReturnsAsync(orderResponseExpected);

              _ratingsServiceMock
                    .Setup(temp => temp.GetRatingsByOrderId(It.IsAny<Guid>()))
                    .ReturnsAsync(ratingsExpected);

              OrderResponse? orderResponseActual =
                    await _orderGetterService.GetOrderByOrderId(orderResponseExpected.OrderId);

              orderResponseActual.Should().NotBeNull();
              orderResponseActual.Should().BeEquivalentTo(orderResponseExpected,
                    options => options.Excluding(x => x.OrderItems));

            foreach (var item in orderResponseActual!.OrderItems!)
            {
                var matchingRating =
                    ratingsExpected.FirstOrDefault(r => r.DishId == item.DishId);

                matchingRating.Should().NotBeNull();
                item.Rating.Should().Be(matchingRating!.Rate);
                item.Comment.Should().Be(matchingRating.Comment);
            }
        }

        [Fact]
        public async Task GetOrdersByUserId_InvalidUserId_ThrowArgumentException()
        {
            _authServiceMock.Setup(temp => temp.GetUserByUserId(It.IsAny<Guid>()))
                .ReturnsAsync((UserDTO?)null);

            Func<Task> action = async () =>
            {
                await _orderGetterService.GetOrdersByUserId(Guid.NewGuid());
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetOrdersByUserId_ValidUserId_ShouldReturnMatchingOrders()
        {
            UserDTO userDTO = _fixture.Build<UserDTO>().Create();
            List<OrderResponse> orderResponsesExpected = new List<OrderResponse>()
            {
                _fixture.Build<OrderResponse>()
                        .With(t=>t.UserId,userDTO.UserId)
                        .With(t=>t.DeliveryAddress,null as AddressResponse)
                        .With(t=>t.OrderItems,null as List<OrderItemResponse>)
                        .Create(),
                _fixture.Build<OrderResponse>()
                        .With(t=>t.UserId,userDTO.UserId)
                        .With(t=>t.DeliveryAddress,null as AddressResponse)
                        .With(t=>t.OrderItems,null as List<OrderItemResponse>)
                        .Create(),
                _fixture.Build<OrderResponse>()
                        .With(t=>t.UserId,userDTO.UserId)
                        .With(t=>t.DeliveryAddress,null as AddressResponse)
                        .With(t=>t.OrderItems,null as List<OrderItemResponse>)
                        .Create(),
            };

            _authServiceMock.Setup(temp => temp.GetUserByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(userDTO);
            _ordersRepositoryMock.Setup(temp => temp.GetOrdersByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(orderResponsesExpected);

            List<OrderResponse>? orderResponsesActual = await _orderGetterService.GetOrdersByUserId(userDTO.UserId);

            orderResponsesActual.Should().NotBeEmpty();
            orderResponsesActual.Should().BeEquivalentTo(orderResponsesExpected);
        }

        [Fact]
        public async Task GetOrders_EmptyOrders_ShouldBeEmpty()
        {
            PaginationRequest paginationRequest = _fixture.Build<PaginationRequest>().Create();

            _ordersRepositoryMock.Setup(temp => temp.GetOrders(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((PaginationResponse<OrderResponse>?)null);

            PaginationResponse<OrderResponse>? orderResponseActual =  await _orderGetterService.GetOrders(paginationRequest);

            orderResponseActual.Should().BeNull();
        }

        [Fact]
        public async Task GetOrders_IfOrdersArePresent_ShouldReturnOrders()
        {
            PaginationRequest paginationRequest = _fixture.Build<PaginationRequest>().Create();

            PaginationResponse<OrderResponse> orderResponseExpected = _fixture.Build<PaginationResponse<OrderResponse>>()
                .With(t=>t.Items,new List<OrderResponse>()
                {
                    _fixture.Build<OrderResponse>().Create(),
                    _fixture.Build<OrderResponse>().Create()
                })
                .With(t=>t.PageSize,paginationRequest.PageSize)
                .With(t=>t.CurrentPage,paginationRequest.Page)
                .Create();

            _ordersRepositoryMock.Setup(temp => temp.GetOrders(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(orderResponseExpected);

            PaginationResponse<OrderResponse>? orderResponseActual  = await _orderGetterService.GetOrders(paginationRequest);

            orderResponseActual.Should().NotBeNull();
            orderResponseActual.Should().BeEquivalentTo(orderResponseExpected);
        }

        [Fact]
        public async Task IsOrderOwnedByUser_IfUserDoNotOwnedOrder_ShouldReturnFalse()
        {
            _ordersRepositoryMock.Setup(temp=>temp.IsOrderOwnedByUser(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            bool isOwned = await _orderGetterService.IsOrderOwnedByUser(Guid.NewGuid(), Guid.NewGuid());
            isOwned.Should().BeFalse();
        }

        [Fact]
        public async Task IsOrderOwnedByUser_IfUserOwnedOrder_ShouldReturnTrue()
        {
            OrderResponse orderResponse = _fixture.Build<OrderResponse>().Create();

            _ordersRepositoryMock.Setup(temp => temp.IsOrderOwnedByUser(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            bool isOwned = await _orderGetterService.IsOrderOwnedByUser(orderResponse.UserId,orderResponse.OrderId);
            isOwned.Should().BeTrue();
        }

        [Fact]
        public async Task IsDishPartOfOrder_IfDishIsNotInOrder_ShouldReturnFalse()
        {
            _ordersRepositoryMock.Setup(temp => temp.IsDishPartOfOrder(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);
            bool isOwned = await _orderGetterService.IsDishPartOfOrder(Guid.NewGuid(), Guid.NewGuid());
            isOwned.Should().BeFalse();
        }

        [Fact]
        public async Task IsDishPartOfOrder_IfDishIsInOrder_ShouldReturnTrue()
        {
            OrderResponse orderResponse = _fixture.Build<OrderResponse>().Create();

            _ordersRepositoryMock.Setup(temp => temp.IsDishPartOfOrder(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);
            bool isOwned = await _orderGetterService.IsDishPartOfOrder(orderResponse.OrderId, orderResponse.OrderItems.FirstOrDefault().DishId);
            isOwned.Should().BeTrue();
        }

        #endregion

        #region CreateOrder

        [Fact]
        public async Task CreateOrder_InvalidUserId_ThrowArgumentException()
        {
            OrderAddRequest orderAddRequest = _fixture.Build<OrderAddRequest>()
                                                .Create();

            var mockImageFile = _fixture.Create<Mock<IFormFile>>();
            mockImageFile.Setup(f => f.FileName).Returns("test.jpg");
            mockImageFile.Setup(f => f.Length).Returns(1024);

            DishResponse dishResponse = _fixture.Build<DishResponse>()
                                    .Create();

            _dishGetterServiceMock.Setup(temp => temp.GetDishByDishId(It.IsAny<Guid>()))
                .ReturnsAsync(dishResponse);

            _authServiceMock.Setup(temp => temp.GetUserByUserId(It.IsAny<Guid>()))
                .ReturnsAsync((UserDTO?)null);

            Func<Task> action = async () =>
            {
                await _orderCreateService.CreateOrder(orderAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateOrder_ValidDetails_ShouldBeSuccessfull()
        {
            OrderAddRequest orderAddRequest = _fixture.Build<OrderAddRequest>()
                                                .Create();

            var mockImageFile = _fixture.Create<Mock<IFormFile>>();
            mockImageFile.Setup(f => f.FileName).Returns("test.jpg");
            mockImageFile.Setup(f => f.Length).Returns(1024);

            DishResponse dishResponse = _fixture.Build<DishResponse>()
                                    .Create();
            AddressResponse addressResponse = _fixture.Build<AddressResponse>()
                                    .With(t => t.UserId, orderAddRequest.UserId)
                                    .Create();
            UserDTO userDTO = _fixture.Build<UserDTO>()
                                .With(t=>t.UserId,orderAddRequest.UserId)
                                .Create();

            Order order = orderAddRequest.ToOrder();
            OrderResponse orderResponseExpected = order.ToOrderResponse();

            _dishGetterServiceMock.Setup(temp => temp.GetDishByDishId(It.IsAny<Guid>()))
                .ReturnsAsync(dishResponse);

            _authServiceMock.Setup(temp => temp.GetUserByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(userDTO);
            _addressServiceMock.Setup(temp=>temp.GetAddressByAddressId(It.IsAny<Guid>()))
                .ReturnsAsync(addressResponse);

            _ordersRepositoryMock.Setup(temp => temp.CreateOrder(It.IsAny<Order>()))
                .ReturnsAsync(order);
            _removeCartItemsServiceMock.Setup(temp => temp.RemoveCartItem(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            OrderResponse orderResponseActual = await _orderCreateService.CreateOrder(orderAddRequest);
            orderResponseExpected.OrderId = orderResponseActual.OrderId;
            orderResponseExpected.TotalBill = orderResponseActual.TotalBill;

            orderResponseActual.Should().NotBeNull();
            orderResponseActual.Should().BeEquivalentTo(orderResponseExpected);
        }

        #endregion

        #region UpdateOrderStatus

        [Fact]
        public async Task UpdateOrderStatusToCancel_CancelOrderWhenCurrentStatusPending_ShouldBeSuccessfull()
           {
            UpdateOrderStatusRequest request = _fixture.Build<UpdateOrderStatusRequest>()
                                                  .With(t => t.OrderStatus, OrderStatus.Cancelled)
                                                  .Create();

            Order order = _fixture.Build<Order>()
                                            .With(t => t.Id, request.OrderId)
                                            .With(t => t.Status, OrderStatus.Pending)
                                            .With(t => t.DeliveryAddress, null as Address)
                                            .With(t => t.OrderItems, null as List<OrderItem>)
                                            .With(t => t.User, null as ApplicationUser)
                                            .With(t=>t.Ratings,null as List<Rating>)
                                            .Create();
            OrderResponse orderResponseExpected = _fixture.Build<OrderResponse>()
                                                .With(t=>t.OrderId,request.OrderId)
                                                .With(t=>t.OrderStatus,request.OrderStatus.ToString())
                                                .With(t=>t.DeliveryAddress,null as AddressResponse)
                                                .With(t=>t.OrderDate,order.OrderDate)
                                                .With(t=>t.TotalBill,order.TotalBill)   
                                            .Create();

            _ordersRepositoryMock.Setup(temp => temp.GetOrderDetailsByOrderId(It.IsAny<Guid>()))
                .ReturnsAsync(order);

            _ordersRepositoryMock.Setup(temp => temp.UpdateOrderStatus(It.IsAny<OrderStatus>(), It.IsAny<Guid>()))
                .ReturnsAsync(orderResponseExpected);

            OrderResponse? orderResponseActual =  await _orderUpdateService.UpdateOrderStatusToCancel(request);
            orderResponseActual.Should().NotBeNull();
            orderResponseActual.Should().BeEquivalentTo(orderResponseExpected);
           }

        [Fact]
        public async Task UpdateOrderStatusToCancel_CancelOrderWhenCurrentStatusConfirmed_ThrowInvalidOperationException()
            {
             UpdateOrderStatusRequest request = _fixture.Build<UpdateOrderStatusRequest>()
                                                    .With(t => t.OrderStatus, OrderStatus.Cancelled)
                                                    .Create();

            Order order = _fixture.Build<Order>()
                                            .With(t=>t.Id,request.OrderId)
                                            .With(t=>t.Status,OrderStatus.Confirmed)
                                            .With(t=>t.DeliveryAddress,null as Address)
                                            .With(t=>t.OrderItems,null as List<OrderItem>)
                                            .With(t=>t.User,null as ApplicationUser)
                                            .With(t => t.Ratings, null as List<Rating>)
                                            .Create();

             _ordersRepositoryMock.Setup(temp => temp.GetOrderDetailsByOrderId(It.IsAny<Guid>()))
                .ReturnsAsync(order);

            Func<Task> action = async () =>
            {
                await _orderUpdateService.UpdateOrderStatusToCancel(request);
            };
            await action.Should().ThrowAsync<InvalidOperationException>();
           }

        [Fact]
        public async Task UpdateOrderStatus_UpdateOrderStatusToCancel_ThrowInvalidOperationException()
        {
            UpdateOrderStatusRequest request = _fixture.Build<UpdateOrderStatusRequest>()
                                                   .With(t => t.OrderStatus, OrderStatus.Cancelled)
                                                   .Create();

            Order order = _fixture.Build<Order>()
                                            .With(t => t.Id, request.OrderId)
                                            .With(t => t.Status, OrderStatus.Pending)
                                            .With(t => t.DeliveryAddress, null as Address)
                                            .With(t => t.OrderItems, null as List<OrderItem>)
                                            .With(t => t.User, null as ApplicationUser)
                                            .With(t => t.Ratings, null as List<Rating>)
                                            .Create();

            _ordersRepositoryMock.Setup(temp => temp.GetOrderDetailsByOrderId(It.IsAny<Guid>()))
               .ReturnsAsync(order);

            Func<Task> action = async () =>
            {
                await _orderUpdateService.UpdateOrderStatus(request);
            };
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateOrderStatus_UpdateOrderStatusToDelivered_ShouldReturnUpdatedDetails()
        {
            UpdateOrderStatusRequest request = _fixture.Build<UpdateOrderStatusRequest>()
                                                   .With(t => t.OrderStatus, OrderStatus.Delivered)
                                                   .Create();

            Order order = _fixture.Build<Order>()
                                            .With(t => t.Id, request.OrderId)
                                            .With(t => t.Status, OrderStatus.Confirmed)
                                            .With(t => t.DeliveryAddress, null as Address)
                                            .With(t => t.OrderItems, null as List<OrderItem>)
                                            .With(t => t.User, null as ApplicationUser)
                                            .With(t => t.Ratings, null as List<Rating>)
                                            .Create();
            OrderResponse orderResponseExpected = _fixture.Build<OrderResponse>()
                                                .With(t => t.OrderId, request.OrderId)
                                                .With(t => t.OrderStatus, request.OrderStatus.ToString())
                                                .With(t => t.DeliveryAddress, null as AddressResponse)
                                                .With(t => t.OrderDate, order.OrderDate)
                                                .With(t => t.TotalBill, order.TotalBill)
                                            .Create();

            _ordersRepositoryMock.Setup(temp => temp.GetOrderDetailsByOrderId(It.IsAny<Guid>()))
               .ReturnsAsync(order);

            _ordersRepositoryMock.Setup(temp => temp.UpdateOrderStatus(It.IsAny<OrderStatus>(), It.IsAny<Guid>()))
                .ReturnsAsync(orderResponseExpected);

            OrderResponse? orderResponseActual = await _orderUpdateService.UpdateOrderStatus(request);
            orderResponseActual.Should().NotBeNull();
            orderResponseActual.Should().BeEquivalentTo(orderResponseExpected);
        }

        #endregion

        #region Reorder

        [Fact]
        public async Task Reorder_WithEmptyOrderId_ShouldThrowArgumentNullException()
        {
            Guid orderId = Guid.Empty;

            Func<Task> action = async () =>
            {
                await _orderGetterService.Reorder(orderId);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Reorder_WithInValidId_ShouldReturnArgumentException()
        {
            Guid orderId = Guid.NewGuid();

            _ordersRepositoryMock
                .Setup(temp => temp.GetOrderByOrderId(It.IsAny<Guid>()))
                .ReturnsAsync((OrderResponse?)null);

            Func<Task> action = async () =>
            {
                await _orderGetterService.Reorder(orderId);
            };

            await action.Should().ThrowAsync<ArgumentException>()
                .WithMessage("OrderId is invalid");
        }

        [Fact]
        public async Task Reorder_WithValidId_ShouldReturnAddedCartItems()
        {
            OrderResponse orderResponseExpected =
                _fixture.Build<OrderResponse>().Create();

            List<CartResponse> cartResponseExpected =
            orderResponseExpected.OrderItems
            .Select(item => new CartResponse
            {
                DishId = item.DishId,
                Quantity = item.Quantity,
                UserId = orderResponseExpected.UserId
            })
            .ToList();

            _ordersRepositoryMock
                .Setup(temp => temp.GetOrderByOrderId(It.IsAny<Guid>()))
                .ReturnsAsync(orderResponseExpected);

            _addCartItemsServiceMock
                .Setup(temp => temp.AddOrderItemsToCart(
                    It.IsAny<List<ReorderCartItemRequest>>()))
                .ReturnsAsync(cartResponseExpected);

            List<CartResponse> cartResponseActual =
                await _orderGetterService.Reorder(orderResponseExpected.OrderId);

            cartResponseActual.Should().NotBeNull();
            cartResponseActual.Count.Should().Be(orderResponseExpected.OrderItems.Count);
        }

        #endregion
    }
}
