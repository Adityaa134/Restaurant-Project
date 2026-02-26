using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Restaurent.Core.DTO;

namespace RestaurentSolution.IntegrationTests
{
    public class AddressControllerIntegrationTests : IntegrationTestBase
    {
        public AddressControllerIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region CreateAddress

        [Fact]
        public async Task CreateAddress_ValidUserId_ShouldReturnCreatedAddressDetails()
        {
            var authenticationResponse = await RegisterAndLoginUser();

            AddressCreateRequest addressCreateRequest = _fixture.Build<AddressCreateRequest>()
                                                        .With(t=>t.UserId,authenticationResponse.UserId)
                                                        .Create();

            var addressResponse = await _httpClient.PostAsJsonAsync("api/Address", addressCreateRequest);
            addressResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            AddressResponse? address = await addressResponse.Content.ReadFromJsonAsync<AddressResponse>();
            address.Should().NotBeNull();
            address.AddressId.Should().NotBeEmpty();
        } 

        [Fact]
        public async Task CreateAddress_InValidUserId_ShouldReturn400badRequest()
        {
            AddressCreateRequest addressCreateRequest = _fixture.Build<AddressCreateRequest>()
                                                        .Create();
            var response= await _httpClient.PostAsJsonAsync("api/Address", addressCreateRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region GetAddressesOfUser

        [Fact]
        public async Task GetAddressesOfUser_ValidUserId_ShouldReturnAddresses()
        {
            var authenticationResponse = await RegisterAndLoginUser();

            List<AddressResponse> expectedAddressList = new List<AddressResponse>()
            {
                await CreateAddress(authenticationResponse.UserId),
                await CreateAddress(authenticationResponse.UserId)
            };

            var userAddressResponse  = await _httpClient.GetAsync($"api/Address/user-address/{authenticationResponse.UserId}");
            userAddressResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            List<AddressResponse>? actualAddressList = await userAddressResponse.Content.ReadFromJsonAsync<List<AddressResponse>>();
            actualAddressList.Should().BeEquivalentTo(expectedAddressList);
        }

        [Fact]
        public async Task GetAddressesOfUser_InValidUserId_ShouldReturn400BadRequests()
        {
            var userAddressResponse = await _httpClient.GetAsync($"api/Address/user-address/{Guid.NewGuid()}");
            userAddressResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region GetAddressById

        [Fact]
        public async Task GetAddressById_ValidAddressId_ShouldReturnAddressDetails()
        {
            var authenticationResponse = await RegisterAndLoginUser();
            var address = await CreateAddress(authenticationResponse.UserId);

            var addressDetails = await _httpClient.GetAsync($"api/Address/{address.AddressId}");

            addressDetails.StatusCode.Should().Be(HttpStatusCode.OK);

            var raw = await addressDetails.Content.ReadAsStringAsync();
            AddressResponse? matchingAddress = await addressDetails.Content.ReadFromJsonAsync<AddressResponse>();
            matchingAddress.Should().BeEquivalentTo(address);

        }
        [Fact]
        public async Task GetAddressById_InValidAddressId_ShouldReturn404NotFound()
        {
            var addressDetails = await _httpClient.GetAsync($"api/Address/{Guid.NewGuid()}");
            addressDetails.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region UpdateAddress

        [Fact]
        public async Task UpdateAddress_InValidAddressId_ShouldReturn400BadRequest()
        {
            AddressUpdateRequest addressUpdateRequest = _fixture.Build<AddressUpdateRequest>().Create();
            var response = await _httpClient.PutAsJsonAsync("api/Address",addressUpdateRequest);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateAddress_ValidAddressIdAndDetails_ShouldReturnUpdatedAddressDetails()
        {
            var authenticationResponse = await RegisterAndLoginUser();

            var address = await CreateAddress(authenticationResponse.UserId);

            AddressUpdateRequest addressUpdateRequest = _fixture.Build<AddressUpdateRequest>()
                                                        .With(t=>t.AddressId,address.AddressId) 
                                                        .Create();

            var actualUpdatedAddress =  await _httpClient.PutAsJsonAsync("/api/Address", addressUpdateRequest);
            actualUpdatedAddress.StatusCode.Should().Be(HttpStatusCode.OK);
            AddressResponse? updatedAddress =  await actualUpdatedAddress.Content.ReadFromJsonAsync<AddressResponse>();
            updatedAddress.Should().NotBeNull();
            updatedAddress.AddressId.Should().Be(addressUpdateRequest.AddressId);
        }

        #endregion
    }
}
