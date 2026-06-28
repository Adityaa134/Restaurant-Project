using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Restaurent.Core.Domain.Entities;
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

        #region FilterDish

        [Fact]
        public async Task FilterDishes_WithValidFilters_ShouldReturnFilteredDishes()
        { 
            DishResponse dish1 = await AddDishToDatabase(price:170,rating:5);
            DishResponse dish2 = await AddDishToDatabase(price: 300, rating: 4);
            DishResponse dish3 = await AddDishToDatabase(price: 200, rating: 4);

            var response = await _httpClient.GetAsync(
                $"api/Dishes/filter?MinPrice=100&MaxPrice=200&MinRating=4");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<DishResponse>? dishes =
                await response.Content.ReadFromJsonAsync<List<DishResponse>>();

            dishes.Should().NotBeNull();
            dishes.Should().HaveCount(2);
            dishes![0].DishId.Should().Be(dish1.DishId);
            dishes![1].DishId.Should().Be(dish3.DishId);
        }

        [Fact]
        public async Task FilterDishes_WithNoMatchingFilters_ShouldReturnEmptyList()
        {
            await AddDishToDatabase(price: 100);

            var response = await _httpClient.GetAsync(
                $"api/Dishes/filter?MinPrice=500");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<DishResponse>? dishes =
                await response.Content.ReadFromJsonAsync<List<DishResponse>>();

            dishes.Should().NotBeNull();
            dishes.Should().BeEmpty();
        }

        [Fact]
        public async Task FilterDishes_WithoutFilters_ShouldReturnAllDishes()
        {
            DishResponse dish1 = await AddDishToDatabase(price: 100);
            DishResponse dish2 = await AddDishToDatabase(price: 250);

            var response = await _httpClient.GetAsync("api/Dishes/filter");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<DishResponse>? dishes =
                await response.Content.ReadFromJsonAsync<List<DishResponse>>();

            dishes.Should().NotBeNull();
            dishes!.Count.Should().BeGreaterThanOrEqualTo(2);
            dishes!.Should().Contain(t => t.DishId == dish1.DishId);
            dishes.Should().Contain(t => t.DishId == dish2.DishId);
        }

        #endregion
    }
}
