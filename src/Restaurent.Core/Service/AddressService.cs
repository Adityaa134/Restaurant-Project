using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Helpers;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class AddressService : IAddressService
    {
        private readonly IAuthService _authService;
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAuthService authService, IAddressRepository addressRepository)
        {
            _authService = authService;
            _addressRepository = addressRepository;
        }

        public async Task<AddressResponse> CreateAddress(AddressCreateRequest request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidator(request);
            UserDTO? userDTO = await _authService.GetUserByUserId(request.UserId);
            if (userDTO == null)
                throw new ArgumentException("Invalid User Id");
            Address address = request.ToAddress();
            address.Id = Guid.NewGuid();
            await _addressRepository.CreateAddress(address);
            return address.ToAddressResponse();
        }

        public async Task<AddressResponse?> GetAddressByAddressId(Guid addressId)
        {
            if(addressId==Guid.Empty)
                throw new ArgumentNullException(nameof(addressId));
            Address? address =  await _addressRepository.GetAddressByAddressId(addressId);
            if (address == null)
                return null;
            return address.ToAddressResponse();
        }

        public async Task<List<AddressResponse>?> GetAddressesByUserId(Guid userId)
        {
            if (userId==Guid.Empty)
                throw new ArgumentNullException("User Id can't be null");
            UserDTO? userDTO = await _authService.GetUserByUserId(userId);
            if (userDTO == null)
                throw new ArgumentException("Invalid User Id");
            List<Address>? addresses =  await _addressRepository.GetAddressesByUserId(userId);
            if (addresses == null)
                return null;
            return addresses.Select(temp=>temp.ToAddressResponse())
                            .ToList();
        }

        public async Task<AddressResponse> UpdateAddress(AddressUpdateRequest request)
        { 
            if(request == null)
                throw new ArgumentNullException(nameof(request));
            ValidationHelper.ModelValidator(request);
            Address? matchingAddress = await _addressRepository.GetAddressByAddressId(request.AddressId);
            if (matchingAddress == null)
                throw new ArgumentException("Invalid Address Id");

            matchingAddress.Area = request.Area;
            matchingAddress.AddressLine = request.AddressLine;
            matchingAddress.City = request.City;
            matchingAddress.Landmark = request.Landmark;

            Address updatedAddress = await _addressRepository.UpdateAddress(matchingAddress);
            return updatedAddress.ToAddressResponse();
        }
    }
}
