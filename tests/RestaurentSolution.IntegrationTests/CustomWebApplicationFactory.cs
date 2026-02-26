using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Restaurent.Core.ServiceContracts;
using Restaurent.Infrastructure.DBContext;

namespace RestaurentSolution.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var inMemorySettings = new Dictionary<string, string>
                {
                    { "Jwt:EXPIRATION_MINUTES", "5" },
                    { "Jwt:Key", "THIS_IS_MY_SECRET_KEY_FOR_JWT_TOKEN" },
                    { "Jwt:Issuer", "http://localhost:2020" },
                    { "Jwt:Audience", "http://localhost:2021" },
                    {"RefreshToken:EXPIRATION_MINUTES","20" }
                };
                config.AddInMemoryCollection(inMemorySettings);
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDBContext>));
                var emailSenderDescriptor = services.SingleOrDefault(temp => temp.ServiceType == typeof(IEmailSenderService));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                if (emailSenderDescriptor != null)
                {
                    services.Remove(emailSenderDescriptor);
                }

                services.AddDbContext<ApplicationDBContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                var emailMock = new Mock<IEmailSenderService>();
                services.AddScoped<IEmailSenderService>(_ => emailMock.Object);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                  .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        }
    }
}
