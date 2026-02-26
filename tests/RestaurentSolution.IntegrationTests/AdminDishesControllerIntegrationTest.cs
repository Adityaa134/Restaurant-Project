using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Restaurent.Core.DTO;

namespace RestaurentSolution.IntegrationTests
{
    public class AdminDishesControllerIntegrationTest : IntegrationTestBase
    {
        public AdminDishesControllerIntegrationTest(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region AddDish

        [Fact]
        public async Task AddDish_AsUser_ShouldReturn403Forbidden()
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Cheese Burger"), "DishName");
            content.Add(new StringContent("89"), "Price");
            content.Add(new StringContent("Cheese Burger"), "Description");
            content.Add(new StringContent(Guid.NewGuid().ToString()), "CategoryId");

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("FakeImage"));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(fileContent, "Dish_Image", "test.jpg");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/AdminDishes/add-dish")
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddDish_AsAdmin_ShouldReturnAddedDishDetails()
        {
            CategoryResponse categoryResponse =  await AddCategoryToDatabase();

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Cheese Burger"), "DishName");
            content.Add(new StringContent("89"), "Price");
            content.Add(new StringContent("Cheese Burger"), "Description");
            content.Add(new StringContent($"{categoryResponse.CategoryId.ToString()}"), "CategoryId");

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("FakeImage"));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(fileContent, "Dish_Image", "test.jpg");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/AdminDishes/add-dish")
            {
                Content = content
            };

            request.Headers.Add("Test-Role", "admin");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            DishResponse? dishResponseActual = await response.Content.ReadFromJsonAsync<DishResponse>();
            dishResponseActual.Should().NotBeNull();
            dishResponseActual.DishId.Should().NotBeEmpty();
            dishResponseActual.CategoryId.Should().Be(categoryResponse.CategoryId);
        }

        #endregion

        #region EditDish

        [Fact]
        public async Task EditDish_AsUser_ShouldReturn403Forbidden()
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(Guid.NewGuid().ToString()), "DishId");
            content.Add(new StringContent("Cheese Burger"), "DishName");
            content.Add(new StringContent("89"), "Price");
            content.Add(new StringContent("Cheese Burger"), "Description");
            content.Add(new StringContent(Guid.NewGuid().ToString()), "CategoryId");

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("FakeImage"));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(fileContent, "Dish_Image", "test.jpg");

            var request = new HttpRequestMessage(HttpMethod.Put, "api/AdminDishes")
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task EditDish_AsAdmin_ShouldReturnUpdatedDishDetails()
        {
            DishResponse dishResponse =  await AddDishByAdmin();
            string newDishName = "Veg Burger";

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(dishResponse.DishId.ToString()), "DishId");
            content.Add(new StringContent(newDishName), "DishName");
            content.Add(new StringContent(dishResponse.Price.ToString()), "Price");
            content.Add(new StringContent($"{dishResponse.Description}"), "Description");
            content.Add(new StringContent($"{dishResponse.CategoryId.ToString()}"), "CategoryId");
            content.Add(new StringContent(dishResponse.Dish_Image_Path), "Image_Path");

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("FakeImage"));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(fileContent, "Dish_Image", "test.jpg");

            var request = new HttpRequestMessage(HttpMethod.Put, "api/AdminDishes")
            {
                Content = content
            };

            request.Headers.Add("Test-Role", "admin");

            var response = await _httpClient.SendAsync(request);
            DishResponse? dishResponseActual =  await response.Content.ReadFromJsonAsync<DishResponse>();
            dishResponseActual.Should().NotBeNull();
            dishResponseActual.DishId.Should().Be(dishResponse.DishId);
            dishResponseActual.CategoryId.Should().Be(dishResponse.CategoryId);
            dishResponseActual.DishName.Should().Be(newDishName);
        }

        #endregion

        #region DeleteDish

        [Fact]
        public async Task DeleteDish_AsUser_ShouldReturn403Forbidden()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/AdminDishes/{Guid.NewGuid()}");
            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteDish_AsAdminIfDishIdValid_ShouldReturnNoContent()
        {
            DishResponse dishResponse = await AddDishByAdmin();

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/AdminDishes/{dishResponse.DishId}");
            request.Headers.Add("Test-Role", "admin");
            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteDish_AsAdminIfDishIdInValid_ShouldReturn404NotFound()
        {            
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/AdminDishes/{Guid.NewGuid()}");
            request.Headers.Add("Test-Role", "admin");
            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        private async Task<DishResponse> AddDishByAdmin()
        {
            CategoryResponse categoryResponse = await AddCategoryToDatabase();

            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Cheese Burger"), "DishName");
            content.Add(new StringContent("89"), "Price");
            content.Add(new StringContent("Cheese Burger"), "Description");
            content.Add(new StringContent($"{categoryResponse.CategoryId.ToString()}"), "CategoryId");

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("FakeImage"));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(fileContent, "Dish_Image", "test.jpg");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/AdminDishes/add-dish")
            {
                Content = content
            };

            request.Headers.Add("Test-Role", "admin");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            DishResponse? dishResponseActual = await response.Content.ReadFromJsonAsync<DishResponse>();
            dishResponseActual.Should().NotBeNull();
            dishResponseActual.DishId.Should().NotBeEmpty();

            return dishResponseActual;
        }
    }
}
