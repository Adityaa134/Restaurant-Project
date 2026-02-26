using AutoFixture;
using FluentAssertions;
using Moq;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.Identity;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Service;
using Restaurent.Core.ServiceContracts;

namespace RestaurentSolution.UnitTests
{
    public class AddressServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IAddressRepository> _addressRepositoryMock;

        private readonly IAddressService _addressService;

        public AddressServiceTests()
        {
            _fixture = new Fixture();
            _addressRepositoryMock = new Mock<IAddressRepository>();
            _authServiceMock = new Mock<IAuthService>();

            _addressService = new AddressService(_authServiceMock.Object, _addressRepositoryMock.Object);
        }


        #region GetAddress

        [Fact]
        public async Task GetAddressByAddressId_NullId_ThrowArgumentNullException()
        {
            Guid? addressId = Guid.Empty;

            Func<Task> action = async () =>
            {
                await _addressService.GetAddressByAddressId(addressId.Value);
            };

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetAddressByAddressId_VaildAddressId_ShouldReturnMatchingAddress()
        {
            Address address = _fixture.Build<Address>()
                                .With(t=>t.User,null as ApplicationUser)
                                .Create();

            AddressResponse addressResponseExpected = address.ToAddressResponse();

            _addressRepositoryMock.Setup(temp => temp.GetAddressByAddressId(It.IsAny<Guid>()))
                .ReturnsAsync(address);

            AddressResponse? addressResponseActual = await _addressService.GetAddressByAddressId(address.Id);

            addressResponseActual.Should().NotBeNull();
            addressResponseActual.Should().BeEquivalentTo(addressResponseExpected);
        }

        [Fact]
        public async Task GetAddressByAddressId_InVaildAddressId_ShouldBeNull()
        {
            _addressRepositoryMock.Setup(temp => temp.GetAddressByAddressId(It.IsAny<Guid>()))
                .ReturnsAsync((Address?)null);

            AddressResponse? addressResponseActual = await _addressService.GetAddressByAddressId(Guid.NewGuid());

            addressResponseActual.Should().BeNull();
        }

        [Fact]
        public async Task GetAddressesByUserId_InvalidUserId_ThrowArgumentException()
        {
            _authServiceMock.Setup(temp => temp.GetUserByUserId(It.IsAny<Guid>()))
                .ReturnsAsync((UserDTO?)null);

            Func<Task> action = async () =>
            {
                await _addressService.GetAddressesByUserId(Guid.NewGuid());
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetAddressesByUserId_ValidUserId_ShouldReturnAddresses()
        {
            UserDTO userDTO = _fixture.Build<UserDTO>()
                .Create();

            List<Address> addresses = new List<Address>() 
            {
                _fixture.Build<Address>()
                                .With(t=>t.User,null as ApplicationUser)
                                .With(t=>t.UserId,userDTO.UserId)
                                .Create(),
                _fixture.Build<Address>()
                                .With(t=>t.User,null as ApplicationUser)
                                .With(t=>t.UserId,userDTO.UserId)
                                .Create(),
                _fixture.Build<Address>()
                                .With(t=>t.User,null as ApplicationUser)
                                .With(t=>t.UserId,userDTO.UserId)
                                .Create()
            };

            List<AddressResponse> addressResponseExpected = addresses.Select(temp=>temp.ToAddressResponse()).ToList();

            _authServiceMock.Setup(temp => temp.GetUserByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(userDTO);

            _addressRepositoryMock.Setup(temp => temp.GetAddressesByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(addresses);

            List<AddressResponse>? addressResponseActual = await _addressService.GetAddressesByUserId(userDTO.UserId);

            addressResponseActual.Should().NotBeEmpty();
            addressResponseActual.Should().BeEquivalentTo(addressResponseExpected);
        }

        #endregion

        #region CreateAddress

        [Fact]
        public async Task CreateAddress_WithInvalidUserId_ThrowArgumentException()
        {
            AddressCreateRequest request = _fixture.Build<AddressCreateRequest>().Create();

            _authServiceMock.Setup(temp => temp.GetUserByUserId(It.IsAny<Guid>()))
                .ReturnsAsync((UserDTO?)null);

            Func<Task> action = async () =>
            {
                await _addressService.CreateAddress(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateAddress_ValidUserId_ShouldBeSuccessfull()
        {
            AddressCreateRequest request = _fixture.Build<AddressCreateRequest>().Create();
            UserDTO userDTO = _fixture.Build<UserDTO>()
                                .With(t=>t.UserId,request.UserId)
                                .Create();

            Address address = request.ToAddress();
            AddressResponse addressResponseExpected = address.ToAddressResponse();

            _authServiceMock.Setup(temp=>temp.GetUserByUserId(It.IsAny<Guid>()))
                .ReturnsAsync((userDTO));

            _addressRepositoryMock.Setup(temp => temp.CreateAddress(It.IsAny<Address>()))
                .ReturnsAsync(address);

            AddressResponse addressResponseActual = await _addressService.CreateAddress(request);

            addressResponseExpected.AddressId = addressResponseActual.AddressId;

            addressResponseActual.Should().NotBeNull();
            addressResponseActual.Should().BeEquivalentTo(addressResponseExpected);
        }

        #endregion

        #region UpdateAddress

        [Fact]
        public async Task UpdateAddress_InvalidAddressId_ArgumentException()
        {
            AddressUpdateRequest request = _fixture.Build<AddressUpdateRequest>().Create();

            _addressRepositoryMock.Setup(temp => temp.GetAddressByAddressId(It.IsAny<Guid>()))
                .ReturnsAsync((Address?)null);

            Func<Task> action = async () =>
            {
                await _addressService.UpdateAddress(request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdateAddress_ProperDetails_ShouldReturnUpdatedAddressDetails()
        {

            Address address = _fixture.Build<Address>()
                .With(t=>t.User,null as ApplicationUser)
                   .Create();

            AddressUpdateRequest addressUpdateRequest = _fixture.Build<AddressUpdateRequest>()
                                  .With(t=>t.AddressId,address.Id)
                                  .With(t => t.AddressLine, address.AddressLine)
                                  .With(t => t.Area, address.Area)
                                  .With(t => t.Landmark, address.Landmark)
                                  .With(t => t.City, address.City)
                                  .With(t=>t.UserId, address.UserId)
                                  .Create();

            AddressResponse addressResponseExpected = address.
                ToAddressResponse();

            _addressRepositoryMock.Setup(temp => temp.GetAddressByAddressId(It.IsAny<Guid>()))
                .ReturnsAsync(address);

            _addressRepositoryMock.Setup(temp => temp.UpdateAddress(It.IsAny<Address>()))
                .ReturnsAsync(address);

            AddressResponse? addressResponseActual = await _addressService.UpdateAddress(addressUpdateRequest);

            addressResponseActual.Should().NotBeNull();
            addressResponseActual.Should().BeEquivalentTo(addressResponseExpected);
        }

        #endregion
    }
}
