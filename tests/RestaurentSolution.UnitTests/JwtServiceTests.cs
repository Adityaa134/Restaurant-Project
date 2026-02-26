using System.IdentityModel.Tokens.Jwt;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.DTO;
using Restaurent.Core.Service;
using Restaurent.Core.ServiceContracts;

namespace RestaurentSolution.UnitTests
{
    public class JwtServiceTests
    {
        private readonly IFixture _fixture;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        private readonly Mock<IAuthService> _authServiceMock;

        public JwtServiceTests()
        {
            _fixture = new Fixture();
            _authServiceMock = new Mock<IAuthService>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:EXPIRATION_MINUTES", "5" },
                { "Jwt:Key", "THIS_IS_MY_SECRET_KEY_FOR_JWT_TOKEN" },
                { "Jwt:Issuer", "http://localhost:2020" },
                { "Jwt:Audience", "http://localhost:2021" },
                {"RefreshToken:EXPIRATION_MINUTES","20" }
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings).Build();

            _jwtService = new JwtService(_configuration,_authServiceMock.Object);
        }

        [Fact]
        public async Task CreateJwtToken_ValidUserDetails_ShouldReturnToken()
        {
            ApplicationUser user = _fixture.Build<ApplicationUser>()
                                    .With(t=>t.Carts,null as List<Carts>)
                                    .With(t => t.Orders, null as List<Order>)
                                    .With(t => t.Addresses, null as List<Address>)
                                    .Create();
            _authServiceMock.Setup(temp => temp.GetUserRole(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("user");

            TokenModel tokenModel =  await _jwtService.CreateJwtToken(user);
            var handler = new JwtSecurityTokenHandler();

            tokenModel.Should().NotBeNull();
            tokenModel.AccessToken.Should().NotBeNull();

            var jwtToken = handler.ReadJwtToken(tokenModel.AccessToken);

            jwtToken.Claims.Should().Contain(c =>
                c.Type == JwtRegisteredClaimNames.Sub &&
                c.Value == user.Id.ToString());

            jwtToken.Claims.Should().Contain(c =>
            c.Type=="role" &&
            c.Value=="user");
        }

        [Fact]
        public async Task CreateJwtToken_NullUser_ShouldThrowArgumentNullException()
        {
            ApplicationUser? user = null;

            Func<Task> action = async () =>
            {
                await _jwtService.CreateJwtToken(user);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
