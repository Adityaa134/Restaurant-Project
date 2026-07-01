using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Restaurent.Core.DTO;
using System.Net;

namespace RestaurentSolution.IntegrationTests
{
    public class CartsControllerIntegrationTests : IntegrationTestBase
    {
        public CartsControllerIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region AddToCart

        [Fact]
        public async Task AddToCart_IfDetailsAreValid_ShouldAddDishToCart()
        {
            AuthenticationResponse authenticationResponse =  await RegisterAndLoginUser();
            DishResponse dishResponse = await AddDishToDatabase();

            AddToCartRequest addToCartRequest = _fixture.Build<AddToCartRequest>()
                                                .With(t=>t.UserId,authenticationResponse.UserId)
                                                .With(t=>t.DishId,dishResponse.DishId)
                                                .Create();
            var response  = await _httpClient.PostAsJsonAsync("api/Carts/add-to-cart", addToCartRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            CartResponse? addToCartResponse = await response.Content.ReadFromJsonAsync<CartResponse>();
            addToCartResponse.Should().NotBeNull();
            addToCartResponse.CartId.Should().NotBeEmpty();
            addToCartResponse.DishId.Should().Be(addToCartRequest.DishId.Value);
            addToCartResponse.UserId.Should().Be(addToCartRequest.UserId.Value);
        }

        [Fact]
        public async Task AddToCart_IfDetailsAreInValid_ShouldReturn400BadRequest()
        {
            AuthenticationResponse authenticationResponse = await RegisterAndLoginUser();

            AddToCartRequest addToCartRequest = _fixture.Build<AddToCartRequest>()
                                                .With(t => t.UserId, authenticationResponse.UserId)
                                                .Create();
            var response = await _httpClient.PostAsJsonAsync("api/Carts/add-to-cart", addToCartRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region GetCartItems

        [Fact]
        public async Task GetCartItems_IfUserHasCartItems_ShouldReturnTheCartItems()
        {
            AuthenticationResponse authenticationResponse = await RegisterAndLoginUser();

            List<CartResponse> addToCartResponseExpected = new List<CartResponse>()
            {
                await AddItemToCart(authenticationResponse.UserId),
                await AddItemToCart(authenticationResponse.UserId)
            };

            var response = await _httpClient.GetAsync($"api/Carts/GetCartItems?userId={authenticationResponse.UserId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<CartResponse>? addToCartResponsActual = await response.Content.ReadFromJsonAsync<List<CartResponse>>();
            addToCartResponsActual.Should().NotBeEmpty();
            addToCartResponsActual.Should().BeEquivalentTo(addToCartResponseExpected);
        }

        [Fact]
        public async Task GetCartItems_IfUserDoesNotHaveCartItems_ShouldReturnEmptyList()
        {
            AuthenticationResponse authenticationResponse = await RegisterAndLoginUser();

            var response = await _httpClient.GetAsync($"api/Carts/GetCartItems?userId={authenticationResponse.UserId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            List<CartResponse>? addToCartResponsActual = await response.Content.ReadFromJsonAsync<List<CartResponse>>();
            addToCartResponsActual.Should().BeEmpty();
        }

        #endregion

        #region UpdateQuantity

        [Fact]
        public async Task UpdateQuantity_IfCartIsInValid_ShouldReturn400BadRequest()
        {
            UpdateQuantityRequest updateQuantityRequest = _fixture.Build<UpdateQuantityRequest>()
                                                          .With(t => t.Quantity, 2)
                                                          .Create();

            var response = await _httpClient.PutAsJsonAsync("api/Carts/update-quantity", updateQuantityRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateQuantity_IfCartIsValid_ShouldReturnUpdatedCartDetails()
        {
            AuthenticationResponse authenticationResponse = await RegisterAndLoginUser();
            CartResponse addToCartResponse = await AddItemToCart(authenticationResponse.UserId);

            UpdateQuantityRequest updateQuantityRequest = _fixture.Build<UpdateQuantityRequest>()
                                                          .With(t=>t.CartId,addToCartResponse.CartId)
                                                          .With(t=>t.Quantity,-1)
                                                          .Create();

            var response = await _httpClient.PutAsJsonAsync("api/Carts/update-quantity", updateQuantityRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            CartResponse? updatedCartResponse = await response.Content.ReadFromJsonAsync<CartResponse>();
            updatedCartResponse.Should().NotBeNull();

            updatedCartResponse.CartId.Should().Be(addToCartResponse.CartId);
            updatedCartResponse.Quantity.Should().Be(updateQuantityRequest.Quantity+addToCartResponse.Quantity);
        }

        #endregion

        #region MergeCart

        [Fact]
        public async Task MergeCart_WithNewItems_ShouldAddItemsToCart()
        {
            DishResponse dishResponse1 =  await AddDishToDatabase();
            DishResponse dishResponse2 = await AddDishToDatabase();

            AuthenticationResponse authenticationResponse =  await RegisterAndLoginUser();

            List<MergeCartRequest> items = new()
            {
                new()
                {
                    DishId = dishResponse1.DishId.Value,
                    Quantity = 2
                },
                new()
                {
                    DishId = dishResponse2.DishId.Value,
                    Quantity = 2
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"api/Carts/{authenticationResponse.UserId}/merge",
                items
            );

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<CartResponse>? cartResponsesActual = await response.Content.ReadFromJsonAsync<List<CartResponse>>();

            cartResponsesActual.Should().NotBeNull();
            cartResponsesActual.Should().HaveCount(items.Count);
        }

        [Fact]
        public async Task MergeCart_WithExistingAndNewItems_ShouldMergeCorrectly()
        {
            DishResponse dishResponse = await AddDishToDatabase();

            AuthenticationResponse authenticationResponse = await RegisterAndLoginUser();

            CartResponse cartResponse =  await AddItemToCart(authenticationResponse.UserId);

            List<MergeCartRequest> items = new()
            {
                new()
                {
                    DishId = cartResponse.DishId,
                    Quantity = 2
                },
                new()
                {
                    DishId = dishResponse.DishId.Value,
                    Quantity = 2
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"/api/Carts/{authenticationResponse.UserId}/merge",
                items
            );

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<CartResponse>? cartResponsesActual = await response.Content.ReadFromJsonAsync<List<CartResponse>>();

            cartResponsesActual.Should().NotBeNull();
            cartResponsesActual.Should().HaveCount(items.Count);
            CartResponse existingItem = cartResponsesActual
                .First(x => x.DishId == cartResponse.DishId);

            CartResponse newItem = cartResponsesActual
                .First(x => x.DishId == dishResponse.DishId);

            existingItem.Quantity.Should().Be(
                items[0].Quantity + cartResponse.Quantity
            );

            newItem.Quantity.Should().Be(items[1].Quantity);
        }

        #endregion

        private async Task<CartResponse> AddItemToCart(Guid userId)
        {
            DishResponse dishResponse = await AddDishToDatabase();

            AddToCartRequest addToCartRequest = _fixture.Build<AddToCartRequest>()
                                                .With(t => t.UserId, userId)
                                                .With(t => t.DishId, dishResponse.DishId)
                                                .With(t=>t.Quantity,1)
                                                .Create();
            var response = await _httpClient.PostAsJsonAsync("api/Carts/add-to-cart", addToCartRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            CartResponse? addToCartResponse = await response.Content.ReadFromJsonAsync<CartResponse>();
            addToCartResponse.Should().NotBeNull();
            addToCartResponse.CartId.Should().NotBeEmpty();

            return addToCartResponse;
        }
    }
}
