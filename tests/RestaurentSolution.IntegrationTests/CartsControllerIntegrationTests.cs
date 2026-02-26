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

            AddToCartResponse? addToCartResponse = await response.Content.ReadFromJsonAsync<AddToCartResponse>();
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

            List<AddToCartResponse> addToCartResponseExpected = new List<AddToCartResponse>()
            {
                await AddItemToCart(authenticationResponse.UserId),
                await AddItemToCart(authenticationResponse.UserId)
            };

            var response = await _httpClient.GetAsync($"api/Carts/GetCartItems?userId={authenticationResponse.UserId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<AddToCartResponse>? addToCartResponsActual = await response.Content.ReadFromJsonAsync<List<AddToCartResponse>>();
            addToCartResponsActual.Should().NotBeEmpty();
            addToCartResponsActual.Should().BeEquivalentTo(addToCartResponseExpected);
        }

        [Fact]
        public async Task GetCartItems_IfUserDoesNotHaveCartItems_ShouldReturnEmptyList()
        {
            AuthenticationResponse authenticationResponse = await RegisterAndLoginUser();

            var response = await _httpClient.GetAsync($"api/Carts/GetCartItems?userId={authenticationResponse.UserId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            List<AddToCartResponse>? addToCartResponsActual = await response.Content.ReadFromJsonAsync<List<AddToCartResponse>>();
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
            AddToCartResponse addToCartResponse = await AddItemToCart(authenticationResponse.UserId);

            UpdateQuantityRequest updateQuantityRequest = _fixture.Build<UpdateQuantityRequest>()
                                                          .With(t=>t.CartId,addToCartResponse.CartId)
                                                          .With(t=>t.Quantity,-1)
                                                          .Create();

            var response = await _httpClient.PutAsJsonAsync("api/Carts/update-quantity", updateQuantityRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            AddToCartResponse? updatedCartResponse = await response.Content.ReadFromJsonAsync<AddToCartResponse>();
            updatedCartResponse.Should().NotBeNull();

            updatedCartResponse.CartId.Should().Be(addToCartResponse.CartId);
            updatedCartResponse.Quantity.Should().Be(updateQuantityRequest.Quantity+addToCartResponse.Quantity);
        }

        #endregion

        #region CheckCartItemExist

        [Fact]
        public async Task CheckCartItemExist_IfCartItemExist_ShouldReturnTrue()
        {
            AuthenticationResponse authenticationResponse =  await RegisterAndLoginUser();
            AddToCartResponse addToCartResponse =  await AddItemToCart(authenticationResponse.UserId);

            var response = await _httpClient.GetAsync($"api/Carts/CheckCartItemExist?userId={authenticationResponse.UserId}&dishId={addToCartResponse.DishId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            bool isExist = await response.Content.ReadFromJsonAsync<bool>();
            isExist.Should().BeTrue();
        }

        [Fact]
        public async Task CheckCartItemExist_IfCartItemDoesNotExist_ShouldReturnFalse()
        {
            AuthenticationResponse authenticationResponse = await RegisterAndLoginUser();

            var response = await _httpClient.GetAsync($"/api/Carts/CheckCartItemExist?userId={authenticationResponse.UserId}&dishId={Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            bool isExist = await response.Content.ReadFromJsonAsync<bool>();
            isExist.Should().BeFalse();
        }

        #endregion

        private async Task<AddToCartResponse> AddItemToCart(Guid userId)
        {
            DishResponse dishResponse = await AddDishToDatabase();

            AddToCartRequest addToCartRequest = _fixture.Build<AddToCartRequest>()
                                                .With(t => t.UserId, userId)
                                                .With(t => t.DishId, dishResponse.DishId)
                                                .With(t=>t.Quantity,1)
                                                .Create();
            var response = await _httpClient.PostAsJsonAsync("api/Carts/add-to-cart", addToCartRequest);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            AddToCartResponse? addToCartResponse = await response.Content.ReadFromJsonAsync<AddToCartResponse>();
            addToCartResponse.Should().NotBeNull();
            addToCartResponse.CartId.Should().NotBeEmpty();

            return addToCartResponse;
        }
    }
}
