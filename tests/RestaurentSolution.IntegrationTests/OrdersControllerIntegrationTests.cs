using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Restaurent.Core.DTO;
using System.Net;
using Restaurent.Core.Enums;

namespace RestaurentSolution.IntegrationTests
{
    public class OrdersControllerIntegrationTests : IntegrationTestBase
    {
        public OrdersControllerIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region CreateOrder 

        [Fact]
        public async Task CreateOrder_InvalidDishId_ShouldReturn400BadRequest()
        {
            OrderAddRequest orderAddRequest = _fixture.Build<OrderAddRequest>().Create();
            var response = await _httpClient.PostAsJsonAsync("api/Orders", orderAddRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateOrder_ValidDetails_ShouldReturnCreatedOrderDetails()
        {
            var authenticationResponse = await RegisterAndLoginUser();
            var address = await CreateAddress(authenticationResponse.UserId);
            DishResponse dishResponse =  await AddDishToDatabase();

            OrderAddRequest orderAddRequest = _fixture.Build<OrderAddRequest>()
                                              .With(t=>t.UserId,authenticationResponse.UserId)
                                              .With(t=>t.DeliveryAddressId,address.AddressId)
                                              .With(t=>t.OrderItems, new List<OrderItemAddRequest>()
                                              {
                                                  new OrderItemAddRequest()
                                                  {
                                                      DishId = dishResponse.DishId,
                                                      Quantity = 2,
                                                      UnitPrice = dishResponse.Price.Value
                                                  }
                                              })
                                              .Create();

            var response = await _httpClient.PostAsJsonAsync("api/Orders", orderAddRequest);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            OrderResponse? orderResponse = await response.Content.ReadFromJsonAsync<OrderResponse>();
            orderResponse.Should().NotBeNull();
            orderResponse.OrderId.Should().NotBeEmpty();
            orderResponse.UserId.Should().Be(authenticationResponse.UserId);
            orderResponse.DeliveryAddress.AddressId.Should().Be(address.AddressId);
        }

        #endregion

        #region GetOrderByOrderId

        [Fact]
        public async Task GetOrderByOrderId_IfOrderExist_ShouldReturnMatchingOrder()
        {
            var authenticationResponse = await RegisterAndLoginUser();
            OrderResponse orderResponse = await CreateOrder(authenticationResponse.UserId);

            var response = await _httpClient.GetAsync($"api/Orders/{orderResponse.OrderId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            OrderResponse? orderResponseActual = await response.Content.ReadFromJsonAsync<OrderResponse>();
            orderResponseActual.Should().NotBeNull();
            orderResponseActual.Should().BeEquivalentTo(orderResponse);
        }

        [Fact]
        public async Task GetOrderByOrderId_IfOrderDoesNotExist_ShouldReturn404NotFound()
        {
            var response = await _httpClient.GetAsync($"api/Orders/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region GetOrdersByUserId

        [Fact]
        public async Task GetOrdersByUserId_IfUserDoesNotHaveOrders_ShouldReturnEmptyList()
        {
            var authenticationResponse = await RegisterAndLoginUser();
            var response = await _httpClient.GetAsync($"api/Orders/user-orders/{authenticationResponse.UserId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            List<OrderResponse>? orderResponses = await response.Content.ReadFromJsonAsync<List<OrderResponse>>();
            orderResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetOrdersByUserId_IfUserHasOrders_ShouldReturnUserOrders()
        {
            var authenticationResponse = await RegisterAndLoginUser();
            List<OrderResponse> orderResponseExpected = new List<OrderResponse>()
            {
                await CreateOrder(authenticationResponse.UserId),
                await CreateOrder(authenticationResponse.UserId)
            };

            var response = await _httpClient.GetAsync($"api/Orders/user-orders/{authenticationResponse.UserId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<OrderResponse>? orderResponsActual = await response.Content.ReadFromJsonAsync<List<OrderResponse>>();
            orderResponsActual.Should().NotBeEmpty();
            orderResponsActual.Should().BeEquivalentTo(orderResponseExpected);
        }

        #endregion

        #region CancelOrder

        [Fact]
        public async Task CancelOrder_IfUpdateStatusIsNotCancel_ShouldReturn400BadRequest()
        {
            UpdateOrderStatusRequest updateOrderStatusRequest = _fixture.Build<UpdateOrderStatusRequest>()
                                                                .With(t => t.OrderStatus, OrderStatus.Delivered)
                                                                .Create();

            var response = await _httpClient.PutAsJsonAsync("api/Orders/cancel-order", updateOrderStatusRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CancelOrder_IfOrderIdIsInvalid_ShouldReturn400BadRequest()
        {
            UpdateOrderStatusRequest updateOrderStatusRequest = _fixture.Build<UpdateOrderStatusRequest>()
                                                                .With(t=>t.OrderStatus,OrderStatus.Cancelled)
                                                                .Create();

            var response = await _httpClient.PutAsJsonAsync("api/Orders/cancel-order", updateOrderStatusRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CancelOrder_IfUpdateStatusIsCancel_ShouldCancelOrder()
        {
            var authenticationResponse = await RegisterAndLoginUser();
            OrderResponse orderResponse = await CreateOrder(authenticationResponse.UserId);

            UpdateOrderStatusRequest updateOrderStatusRequest = _fixture.Build<UpdateOrderStatusRequest>()
                                                        .With(t=>t.OrderId, orderResponse.OrderId)  
                                                        .With(t => t.OrderStatus, OrderStatus.Cancelled)
                                                        .Create();

            var response = await _httpClient.PutAsJsonAsync("api/Orders/cancel-order", updateOrderStatusRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            OrderResponse? orderResponseActual = await response.Content.ReadFromJsonAsync<OrderResponse>();
            orderResponseActual.Should().NotBeNull();
            orderResponseActual.OrderStatus.Should().Be(OrderStatus.Cancelled.ToString());
        }

        #endregion

        #region GetOrders

        [Fact]
        public async Task GetOrders_AsAdmin_ShouldReturnAllUsersOrdersIfOrdersExist()
        {
            var user1 = await RegisterAndLoginUser();
            var user2 = await RegisterAndLoginUser();
            List<OrderResponse> orderResponseExpected = new List<OrderResponse>()
            {
                await CreateOrder(user1.UserId),
                await CreateOrder(user1.UserId),
                await CreateOrder(user2.UserId),
            };

            var request = new HttpRequestMessage(HttpMethod.Get,$"/api/Orders?Page={1}&PageSize={5}");
            request.Headers.Add("Test-Role", "admin");

            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            PaginationResponse<OrderResponse>? paginationOrderResponse = await response.Content.ReadFromJsonAsync<PaginationResponse<OrderResponse>>();
            paginationOrderResponse.Items.Should().NotBeEmpty();
            paginationOrderResponse.Items.Should().HaveCount(orderResponseExpected.Count);
            paginationOrderResponse.Items.Should().BeEquivalentTo(orderResponseExpected);
        }

        [Fact]
        public async Task GetOrders_AsUser_ShouldReturn403Forbidden()
        {
            var response = await _httpClient.GetAsync($"/api/Orders?Page={1}&PageSize={5}");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        #endregion

        #region UpdateOrderStatus

        [Fact]
        public async Task UpdateOrderStatus_AsUser_ShouldReturn403Forbidden()
        {
            UpdateOrderStatusRequest updateOrderStatusRequest = _fixture.Build<UpdateOrderStatusRequest>()
                .With(t=>t.OrderStatus,OrderStatus.Confirmed).Create();

            var response = await _httpClient.PutAsJsonAsync("api/Orders",updateOrderStatusRequest);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateOrderStatus_AsAdmin_ShouldReturnUpdatedOrderStatusDetails()
        {
            var user = await RegisterAndLoginUser();
            OrderResponse orderResponse =  await CreateOrder(user.UserId);

            UpdateOrderStatusRequest updateOrderStatusRequest = _fixture.Build<UpdateOrderStatusRequest>()
                .With(t=>t.OrderId,orderResponse.OrderId)
                .With(t => t.OrderStatus, OrderStatus.Confirmed)
                .Create();

            var request =  new HttpRequestMessage(HttpMethod.Put, "api/Orders");
            request.Content = JsonContent.Create(updateOrderStatusRequest);
            request.Headers.Add("Test-Role", "admin");

            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            OrderResponse? orderResponseActual = await response.Content.ReadFromJsonAsync<OrderResponse>();
            orderResponseActual.OrderId.Should().Be(orderResponse.OrderId);
            orderResponseActual.OrderStatus.Should().Be(updateOrderStatusRequest.OrderStatus.ToString());
        }

        #endregion

        private async Task<OrderResponse> CreateOrder(Guid userId)
        {
            var address = await CreateAddress(userId);
            DishResponse dishResponse = await AddDishToDatabase();

            OrderAddRequest orderAddRequest = _fixture.Build<OrderAddRequest>()
                                              .With(t => t.UserId, userId)
                                              .With(t => t.DeliveryAddressId, address.AddressId)
                                              .With(t => t.OrderItems, new List<OrderItemAddRequest>()
                                              {
                                                  new OrderItemAddRequest()
                                                  {
                                                      DishId = dishResponse.DishId,
                                                      Quantity = 2,
                                                      UnitPrice = dishResponse.Price.Value
                                                  }
                                              })
                                              .Create();

            var orderCreatedresponse = await _httpClient.PostAsJsonAsync("api/Orders", orderAddRequest);
            orderCreatedresponse.StatusCode.Should().Be(HttpStatusCode.Created);

            OrderResponse? orderResponseExpected = await orderCreatedresponse.Content.ReadFromJsonAsync<OrderResponse>();
            orderResponseExpected.Should().NotBeNull();
            orderResponseExpected.OrderId.Should().NotBeEmpty();

            return orderResponseExpected;
        }
    }
}
