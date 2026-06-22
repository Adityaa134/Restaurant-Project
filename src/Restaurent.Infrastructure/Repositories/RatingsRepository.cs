using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Infrastructure.DBContext;

namespace Restaurent.Infrastructure.Repositories
{
    public class RatingsRepository : IRatingsRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public RatingsRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Rating> AddRating(Rating rating)
        {
            await _dbContext.Ratings.AddAsync(rating);
            await _dbContext.SaveChangesAsync();
            return rating;
        }
        public async Task<List<Rating>?> GetRatingsByOrderId(Guid orderId)
        {
            return await _dbContext.Ratings
                .AsNoTracking()
                .Where(r => r.OrderId == orderId)
                .ToListAsync();
        }
        public async Task<bool> IsRatingGiven(Guid orderId,Guid dishId)
        {
            return await _dbContext.Ratings
                .AnyAsync(t => t.OrderId == orderId && t.DishId == dishId);
        }
    }
}
