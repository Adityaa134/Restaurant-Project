using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IAddressService
    {
        /// <summary>
        /// Creates new address in the data store
        /// </summary>
        /// <param name="request">Address to create</param>
        /// <returns>Returns the address </returns>
        Task<AddressResponse> CreateAddress(AddressCreateRequest request);

        /// <summary>
        /// Updates the address in the data store
        /// </summary>
        /// <param name="request">Updated Address</param>
        /// <returns>Returns updated address</returns>
        Task<AddressResponse> UpdateAddress(AddressUpdateRequest request);

        /// <summary>
        /// Search the address based on address id 
        /// </summary>
        /// <param name="addressId">The address id to search</param>
        /// <returns>Returns the address based on address id </returns>
        Task<AddressResponse?> GetAddressByAddressId(Guid addressId);

        /// <summary>
        /// Returns all the addresses of user 
        /// </summary>
        /// <param name="userId">The user</param>
        /// <returns>Returns addresses of user</returns>
        Task<List<AddressResponse>?> GetAddressesByUserId(Guid userId);
    }
}
