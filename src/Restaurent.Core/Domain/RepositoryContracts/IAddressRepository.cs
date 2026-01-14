using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.Domain.RepositoryContracts
{
    public interface IAddressRepository
    {
        /// <summary>
        /// Creates new address in the data store
        /// </summary>
        /// <param name="address">Address to create</param>
        /// <returns>Returns the address </returns>
        Task<Address> CreateAddress(Address address);

        /// <summary>
        /// Updates the address in the data store
        /// </summary>
        /// <param name="address">Updated Address</param>
        /// <returns>Returns updated address</returns>
        Task<Address> UpdateAddress(Address address);

        /// <summary>
        /// Returns all the addresses of user 
        /// </summary>
        /// <param name="userId">The user</param>
        /// <returns>Returns addresses of user</returns>
        Task<List<Address>?> GetAddressesByUserId(Guid userId);

        /// <summary>
        /// Search the address based on address id 
        /// </summary>
        /// <param name="addressId">The address id to search</param>
        /// <returns>Returns the address based on address id </returns>
        Task<Address?> GetAddressByAddressId(Guid addressId);
    }
}
