using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Infrastructure.DBContext;

namespace Restaurent.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public AddressRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Address> CreateAddress(Address address)
        {
            await _dbContext.Address.AddAsync(address);
            await _dbContext.SaveChangesAsync();
            return address;
        }

        public async Task<Address?> GetAddressByAddressId(Guid addressId)
        {
           return await _dbContext.Address
                                  .FirstOrDefaultAsync(temp=>temp.Id == addressId);
        }

        public async Task<List<Address>?> GetAddressesByUserId(Guid userId)
        {
            return await _dbContext.Address
                                   .Where(a=>a.UserId == userId)
                                   .Include(a=>a.User)
                                   .AsNoTracking()
                                   .ToListAsync();
        }

        public async Task<Address> UpdateAddress(Address address)
        {
            Address? matchingAddress =  await GetAddressByAddressId(address.Id);
            if (matchingAddress == null)
                return address;

            matchingAddress.Area = address.Area;
            matchingAddress.AddressLine = address.AddressLine;
            matchingAddress.City = address.City;
            matchingAddress.Landmark = address.Landmark;
            await _dbContext.SaveChangesAsync();
            return matchingAddress;
        }
    }
}
