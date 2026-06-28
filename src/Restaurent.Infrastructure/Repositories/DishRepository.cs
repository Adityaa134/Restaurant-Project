using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Infrastructure.DBContext;

namespace Restaurent.Infrastructure.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public DishRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Dish> AddDish(Dish dish)
        {
            await _dbContext.Dishes
                      .AddAsync(dish);
            await _dbContext.SaveChangesAsync();
            return await GetDishByDishId(dish.DishId);
        }

        public async Task<bool> DeleteDishByDishId(Guid dishId)
        {
            Dish? matchingDish = await GetDishByDishId(dishId);
            if (matchingDish != null)
            {
                _dbContext.Dishes.Remove(matchingDish);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Dish>> GetAllDishes()
        {
            return await _dbContext.Dishes
                                   .Include(t=>t.Category)
                                   .Where(c=>c.Category.Status==true)
                                   .AsNoTracking()
                                   .ToListAsync();
        }

        public async Task<Dish?> GetDishByDishId(Guid dishId)
        {
            Dish? matchingDish = await _dbContext.Dishes
                                                 .Include(t => t.Category)
                                                 .FirstOrDefaultAsync(temp=>temp.DishId == dishId);
            if (matchingDish == null)
                return null;
            return matchingDish;
        }

        public async Task<List<Dish>> GetDishesBasedOnCategoryId(Guid categoryId)
        {
            return await _dbContext.Dishes
                                   .Where(temp=>temp.Category.Id == categoryId)
                                   .Include(t=>t.Category)
                                   .AsNoTracking()
                                   .ToListAsync();
        }

        public async Task<List<Dish>?> SearchDish(string searchString)
        {
           return await _dbContext.Dishes
                                  .Where(dish=> EF.Functions.Like(dish.DishName.ToLower(),$"%{searchString.ToLower()}%"))
                                   .Include(t=>t.Category)
                                   .AsNoTracking()
                                   .ToListAsync();
                                  
        }

        public async Task<Dish> UpdateDish(Dish dish)
        {
            Dish? matchingDish = await GetDishByDishId(dish.DishId);
            if (matchingDish == null)
                return dish;
            matchingDish.DishName = dish.DishName;
            matchingDish.Description = dish.Description;
            matchingDish.Price = dish.Price;
            matchingDish.Image_Path = dish.Image_Path;

            await _dbContext.SaveChangesAsync();
            return matchingDish;
        }

        public async Task<Dish?> ApplyNewRatingToDish(Rating rating)
        {
            Dish? dish = await GetDishByDishId(rating.DishId);
            if (dish == null) return null;
            int newTotal = dish.TotalRatings + 1;
            decimal avgRating = ((dish.AverageRating*dish.TotalRatings) + rating.Rate) / newTotal;
            dish.TotalRatings = newTotal;
            dish.AverageRating = avgRating;
            await _dbContext.SaveChangesAsync();
            return dish;
        }

        public async Task<bool> IsDishExist(Guid dishId)
        {
            return await _dbContext.Dishes.AnyAsync(t=>t.DishId ==  dishId);
        }

        public async Task<List<Dish>?> FilterDishes(DishFilterRequest request)
        {
            IQueryable<Dish> query = _dbContext.Dishes.AsQueryable();

            if (request.MinPrice.HasValue)
            {
                query = query.Where(t => t.Price >= request.MinPrice);
            }
            if (request.MaxPrice.HasValue)
            {
                query = query.Where(t => t.Price <= request.MaxPrice);
            }
            if (request.MinRating.HasValue)
            {
                query = query.Where(t => t.AverageRating >= request.MinRating);
            }

            return await query.ToListAsync();
        }
    }
}
