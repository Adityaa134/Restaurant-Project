using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.DTO;
using Restaurent.Infrastructure.DBContext;

namespace RestaurentSolution.IntegrationTests
{
    public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>,IAsyncLifetime
    {
        protected readonly HttpClient _httpClient;
        protected readonly CustomWebApplicationFactory _factory;
        protected readonly IFixture _fixture;

        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
            _fixture = new Fixture();
        }

        private async Task  ResetDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }

        protected async Task<HttpResponseMessage> ConfirmEmail(string email)
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser? user = await userManager.FindByEmailAsync(email);

            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            return await _httpClient.PostAsync($"/api/Account/confirm-email-success?uid={user.Id}&token={token}", null);
        }

        protected async Task<AuthenticationResponse> RegisterAndLoginUser()
        {
            var unique = Guid.NewGuid().ToString("N").Substring(0,5);
            var username = $"U{unique}";
            var email = $"test{unique}@gmail.com";

            RegisterRequestt request = _fixture.Build<RegisterRequestt>()
                                        .With(t => t.Password, "TestP@123")
                                        .With(t => t.ConfirmPassword, "TestP@123")
                                        .With(t => t.Email, email)
                                        .With(t => t.UserName, username)
                                        .With(t => t.PhoneNumber, "1234567890")
                                        .Create();

            var response = await _httpClient.PostAsJsonAsync("api/Account/register", request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var confirmEmailResponse = await ConfirmEmail(request.Email);
            confirmEmailResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            LoginRequestt loginRequest = _fixture.Build<LoginRequestt>()
                                        .With(t => t.Password, "TestP@123")
                                        .With(t => t.UserName, username)
                                        .Create();

            var loginResponse = await _httpClient.PostAsJsonAsync("api/Account/login", loginRequest);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

           return await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
        }

        protected async Task<AddressResponse> CreateAddress(Guid userId)
        {
            AddressCreateRequest addressCreateRequest = _fixture.Build<AddressCreateRequest>()
                                                        .With(t => t.UserId,userId)
                                                        .Create();

            var addressResponse = await _httpClient.PostAsJsonAsync("api/Address", addressCreateRequest);
            addressResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            AddressResponse? address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
            address.Should().NotBeNull();
            address.AddressId.Should().NotBeEmpty();

            return address;
        }

        protected async Task<DishResponse> AddDishToDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            Category category = _fixture.Build<Category>()
                                 .With(t=>t.Status,true)
                                 .With(t=>t.Dishes,null as List<Dish>)
                                 .Create();

            Dish dish = _fixture.Build<Dish>()
                            .With(t => t.Category, category)
                            .With(t=>t.CategoryId,category.Id)
                            .With(t=>t.DishName,"Cheese Pizza")
                            .With(t => t.CartItems, null as List<Carts>)
                            .With(t => t.OrderItems, null as List<OrderItem>)
                            .Create();

            await db.Dishes.AddAsync(dish);
            await db.SaveChangesAsync();

            return dish.ToDishResponse();
        }

        protected async Task<CategoryResponse> AddCategoryToDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            Category category = _fixture.Build<Category>()
                                 .With(t => t.Dishes, null as List<Dish>)
                                 .Create();

            await db.Categories.AddAsync(category);
            await db.SaveChangesAsync();

            return category.ToCategoryResponse();
        }

        public async Task InitializeAsync() 
        {
            await ResetDatabase();  
        }

        public Task DisposeAsync() 
        {
            return Task.CompletedTask;
        }
    }
}
