using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Restaurent.Core.DTO;


namespace RestaurentSolution.IntegrationTests
{
    public class DishesControllerIntegrationTest : IntegrationTestBase
    {
        public DishesControllerIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region GetDishes

        [Fact]
        public async Task GetDishes_IfNoDishesExist_ShouldReturnEmptyList()
        {
            var response = await _httpClient.GetAsync("api/Dishes");

            List<DishResponse>? dishes = await response.Content.ReadFromJsonAsync<List<DishResponse>>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            dishes.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDishes_IfDishesExist_ShouldReturnDishesList()
        {
            List<DishResponse> dishResponsesExpected = new List<DishResponse>()
            {
                await AddDishToDatabase(),
                await AddDishToDatabase()
            };
            
            var response = await _httpClient.GetAsync("api/Dishes");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            List<DishResponse>? dishResponsesActual = await response.Content.ReadFromJsonAsync<List<DishResponse>>();
            dishResponsesActual.Should().NotBeEmpty();
            dishResponsesActual.Should().BeEquivalentTo(dishResponsesExpected);
        }

        #endregion

        #region GetDishByDishId

        [Fact]
        public async Task GetDishByDishId_IfDishDoesNotExist_ShouldReturn404NotFound()
        {
            var response = await _httpClient.GetAsync($"api/Dishes/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetDishByDishId_IfDishExist_ShouldReturnMatchingDish()
        {
           DishResponse dishResponseExpected =  await AddDishToDatabase();
            var response = await _httpClient.GetAsync($"api/Dishes/{dishResponseExpected.DishId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            DishResponse? dishResponseActual = await response.Content.ReadFromJsonAsync<DishResponse>();
            dishResponseActual.Should().NotBeNull();
            dishResponseActual.Should().BeEquivalentTo(dishResponseExpected);
        }

        #endregion

        #region SearchDish

        [Fact]
        public async Task SearchDish_IfNoDishFound_ShouldReturnEmptyList()
        {
            var response = await _httpClient.GetAsync("api/Dishes/pizza");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<DishResponse>? dishResponses = await response.Content.ReadFromJsonAsync<List<DishResponse>>();
            dishResponses.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchDish_IfDishFound_ShouldReturnMatchingDishesList()
        {
            DishResponse dishResponse =  await AddDishToDatabase();
            List<DishResponse> dishResponsesExpected = new List<DishResponse>()
            {
                dishResponse
            };
            var response = await _httpClient.GetAsync($"api/Dishes/{dishResponse.DishName}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<DishResponse>? dishResponsesActual = await response.Content.ReadFromJsonAsync<List<DishResponse>>();
            dishResponsesActual.Should().NotBeEmpty();
            dishResponsesActual.Should().BeEquivalentTo(dishResponsesExpected);
        }

        #endregion
    }
}
