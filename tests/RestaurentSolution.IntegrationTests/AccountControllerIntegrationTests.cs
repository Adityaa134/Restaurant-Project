using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Restaurent.Core.DTO;

namespace RestaurentSolution.IntegrationTests
{
    public class AccountControllerIntegrationTests : IntegrationTestBase
    {
        public AccountControllerIntegrationTests(CustomWebApplicationFactory factory) :base(factory)
        {
        }

        #region PostRegister

        [Fact]
        public async Task PostRegister_ValidDetails_ShouldReturn403VerifyEmailIdToLogin()
        {
            RegisterRequestt request = _fixture.Build<RegisterRequestt>()
                                        .With(t=>t.Password,"TestP@123")
                                        .With(t => t.ConfirmPassword, "TestP@123")
                                        .With(t => t.Email, "test@gmail.com")
                                        .With(t => t.UserName, "TestP_123")
                                        .With(t=>t.PhoneNumber, "1234567890")
                                        .Create();

            var response = await _httpClient.PostAsJsonAsync("api/Account/register", request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails
                >();

            problem.Status.Should().Be(403);
            problem.Detail.Should().Be("Please verify your emailId to login");
        }

        #endregion

        #region Login

        [Fact]
        public async Task Login_ValidCredentails_ShouldReturn403IfUserEmailIsNotVerified()
        {
            RegisterRequestt request = _fixture.Build<RegisterRequestt>()
                                        .With(t => t.Password, "TestP@123")
                                        .With(t => t.ConfirmPassword, "TestP@123")
                                        .With(t => t.Email, "test@gmail.com")
                                        .With(t => t.UserName, "TestP_123")
                                        .With(t => t.PhoneNumber, "1234567890")
                                        .Create();

            var response = await _httpClient.PostAsJsonAsync("api/Account/register", request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            LoginRequestt loginRrequest = _fixture.Build<LoginRequestt>()
                                        .With(t => t.Password, "TestP@123")
                                        .With(t => t.UserName, "TestP_123")
                                        .Create();

            var loginResponse = await _httpClient.PostAsJsonAsync("api/Account/login", loginRrequest);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var problem = await loginResponse.Content.ReadFromJsonAsync<ProblemDetails
                >();

            problem.Status.Should().Be(403);
            problem.Detail.Should().Be("Please verify your emailId to login");
        }
       

        [Fact]
        public async Task Login_InValidCredentails_ShouldReturn401()
        {
            RegisterRequestt request = _fixture.Build<RegisterRequestt>()
                                        .With(t => t.Password, "TestP@123")
                                        .With(t => t.ConfirmPassword, "TestP@123")
                                        .With(t => t.Email, "test@gmail.com")
                                        .With(t => t.UserName, "TestP_123")
                                        .With(t => t.PhoneNumber, "1234567890")
                                        .Create();

            var response = await _httpClient.PostAsJsonAsync("api/Account/register", request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            //Confirm email
            var confirmEmailResponse = await ConfirmEmail(request.Email);
            confirmEmailResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            LoginRequestt loginRequest = _fixture.Build<LoginRequestt>()
                                        .With(t => t.Password, "Testp@123")
                                        .With(t => t.UserName, "Test_p123")
                                        .Create();

            var loginResponse = await _httpClient.PostAsJsonAsync("api/Account/login", loginRequest);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var problem = await loginResponse.Content.ReadFromJsonAsync<ProblemDetails
                >();

            problem.Status.Should().Be(401);
            problem.Detail.Should().Be("Invalid Username or password");
        }

        [Fact]
        public async Task Login_ValidCredentials_ShouldBeSuccessfull()
        {
            RegisterRequestt request = _fixture.Build<RegisterRequestt>()
                                        .With(t => t.Password, "TestP@123")
                                        .With(t => t.ConfirmPassword, "TestP@123")
                                        .With(t => t.Email, "test@gmail.com")
                                        .With(t => t.UserName, "TestP_123")
                                        .With(t => t.PhoneNumber, "1234567890")
                                        .Create();

            var response = await _httpClient.PostAsJsonAsync("api/Account/register", request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            //Confirm email
            var confirmEmailResponse = await ConfirmEmail(request.Email);
            confirmEmailResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            LoginRequestt loginRequest = _fixture.Build<LoginRequestt>()
                                        .With(t => t.Password, "TestP@123")
                                        .With(t => t.UserName, "TestP_123")
                                        .Create();

            var loginResponse = await _httpClient.PostAsJsonAsync("api/Account/login", loginRequest);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var authenticationResponse = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
            authenticationResponse.Should().NotBeNull();
            authenticationResponse.UserName.Should().BeEquivalentTo(loginRequest.UserName);
        }

        #endregion

        #region ConfirmEmail

        [Fact]
        public async Task ConfirmEmail_IfEmailExist_ShouldConfirmEmail()
        {
            RegisterRequestt request = _fixture.Build<RegisterRequestt>()
                                        .With(t => t.Password, "TestP@123")
                                        .With(t => t.ConfirmPassword, "TestP@123")
                                        .With(t => t.Email, "test@gmail.com")
                                        .With(t => t.UserName, "TestP_123")
                                        .With(t => t.PhoneNumber, "1234567890")
                                        .Create();

            var response = await _httpClient.PostAsJsonAsync("api/Account/register", request);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var confirmEmailResponse = await ConfirmEmail(request.Email);
            confirmEmailResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var message = await confirmEmailResponse.Content.ReadFromJsonAsync<string>();
            message.Should().BeEquivalentTo("Email Verified");
        }

        #endregion
    }
}
