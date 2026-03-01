using AutoFixture;
using FluentAssertions;
using Restaurent.Core.DTO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace RestaurentSolution.IntegrationTests
{
    public class CategoriesControllerIntegrationTests : IntegrationTestBase
    {
        public CategoriesControllerIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region GetAllCategories

        [Fact]
        public async Task GetAllCategories_IfCatrgoriesExist_ShouldReturnAllCategoriesWhichStatusIsTrue()
        {
            List<CategoryResponse> categoryResponse = new List<CategoryResponse>()
            {
                await AddCategoryToDatabase(),
                await AddCategoryToDatabase(),
                await AddCategoryToDatabase()
            };

            var response = await _httpClient.GetAsync("api/Categories");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            List<CategoryResponse>? categoryResponseActual = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();

            categoryResponseActual.Should().NotBeEmpty();

            categoryResponseActual.Should().OnlyContain(s => s.Status == true);
        }

        [Fact]
        public async Task GetAllCategories_IfCatrgoriesDoesNotExist_ShouldReturnEmptyList()
        {
            var response = await _httpClient.GetAsync("api/Categories");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            List<CategoryResponse>? categoryResponse = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
            categoryResponse.Should().BeEmpty();
        }

        #endregion

        #region GetCategoryById

        [Fact]
        public async Task GetCategoryById_IfCategoryIdNotFound_ShouldReturn404NotFound()
        {
            var response = await _httpClient.GetAsync($"api/Categories/{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCategoryById_IfCategoryIdFound_ShouldReturnMatchingCategory()
        {
            CategoryResponse categoryResponseExpected = await AddCategoryToDatabase();

            var response = await _httpClient.GetAsync($"api/Categories/{categoryResponseExpected.CategoryId}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            CategoryResponse? categoryResponseActual = await response.Content.ReadFromJsonAsync<CategoryResponse>();
            categoryResponseActual.Should().NotBeNull();
            categoryResponseActual.Should().BeEquivalentTo(categoryResponseExpected);
        }

        #endregion

        #region AddCategory

        [Fact]
        public async Task AddCategory_AsUser_ShouldReturn403Forbidden()
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent("Burger"), "CategoryName");
            content.Add(new StringContent("true"), "Status");

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("FakeImage"));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(fileContent, "Cat_Image", "test.jpg");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/Categories/add-category")
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddCategory_AsAdmin_ShouldReturnAddedCategoryDetails()
        {
            var content =  new MultipartFormDataContent();
            content.Add(new StringContent("Burger"), "CategoryName");
            content.Add(new StringContent("true"), "Status");

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("FakeImage"));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            content.Add(fileContent, "Cat_Image", "test.jpg");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/Categories/add-category")
            {
                Content = content
            };

            request.Headers.Add("Test-Role", "admin");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            CategoryResponse? categoryResponseActual = await response.Content.ReadFromJsonAsync<CategoryResponse>();
            categoryResponseActual.Should().NotBeNull();
            categoryResponseActual.CategoryId.Should().NotBeEmpty();
            categoryResponseActual.Status.Should().BeTrue();
        }

        #endregion

        #region UpdateCategoryStatus

        [Fact]
        public async Task UpdateCategoryStatus_AsUser_ShouldReturn403Forbidden()
        {
            CategoryStatusUpdateRequest categoryStatusUpdateRequest = _fixture.Build<CategoryStatusUpdateRequest>().Create();
            var response = await _httpClient.PutAsJsonAsync("api/Categories",categoryStatusUpdateRequest);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateCategoryStatus_AsAdmin_ShouldReturnUpdatedCategoryDetails()
        {
            CategoryResponse categoryResponseExpected =  await AddCategoryToDatabase();
            CategoryStatusUpdateRequest categoryStatusUpdateRequest = _fixture.Build<CategoryStatusUpdateRequest>()
                .With(t=>t.CategoryId, categoryResponseExpected.CategoryId)
                .With(t=>t.Status,!categoryResponseExpected.Status)
                .Create();

            var request = new HttpRequestMessage(HttpMethod.Put, "api/Categories");
            request.Content = JsonContent.Create(categoryStatusUpdateRequest);
            request.Headers.Add("Test-Role", "admin");

            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            CategoryResponse? categoryResponseActual = await response.Content.ReadFromJsonAsync<CategoryResponse>();
            categoryResponseActual.Should().NotBeNull();
            categoryResponseActual.CategoryId.Should().Be(categoryResponseExpected.CategoryId);
            categoryResponseActual.Status.Should().Be(categoryStatusUpdateRequest.Status);
        }

        #endregion

        #region GetAllCategoriesAdmin

        [Fact]
        public async Task GetAllCategoriesAdmin_AsUser_ShoulsReturn403Forbidden()
        {
            var response = await _httpClient.GetAsync("api/Categories/admin/categories");
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAllCategoriesAdmin_AsAdmin_ShoulsReturnAllCategoriesOfBothStatus()
        {
            List<CategoryResponse> categoryResponseExpected = new List<CategoryResponse>()
            {
                await AddCategoryToDatabase(),
                await AddCategoryToDatabase(),
                await AddCategoryToDatabase()
            };

            var request = new HttpRequestMessage(HttpMethod.Get, "api/Categories/admin/categories");
            request.Headers.Add("Test-Role", "admin");

            var response = await _httpClient.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            List<CategoryResponse>? categoryResponseActual = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
            categoryResponseActual.Should().NotBeEmpty();
            categoryResponseActual.Should().BeEquivalentTo(categoryResponseExpected);
        }

        #endregion
       
    }
}
