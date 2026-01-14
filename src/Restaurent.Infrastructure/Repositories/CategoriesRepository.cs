using System;
using Microsoft.EntityFrameworkCore;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Infrastructure.DBContext;

namespace Restaurent.Infrastructure.Repositories
{
    public class CategoriesRepository : ICategoryRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CategoriesRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category> AddCategory(Category? category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _dbContext.Categories
                                   .Where(c=>c.Status==true)
                                   .AsNoTracking()
                                   .ToListAsync();  
        }

        public async Task<List<Category>> GetAllCategoriesAdmin()
        {
            return await _dbContext.Categories
                                   .AsNoTracking()
                                   .ToListAsync();
        }

        public async Task<Category?> GetCategoryByCategoryId(Guid categoryId)
        {
            Category? matchingCategory = await _dbContext.Categories
                                                         .FirstOrDefaultAsync(temp=>temp.Id == categoryId);
            if (matchingCategory == null)
                return null;
            return matchingCategory;
        }

        public async Task<Category?> UpdateCategoryStatus(bool status, Guid categoryId)
        {
           Category? matchingCategory =  await GetCategoryByCategoryId(categoryId);
            if(matchingCategory == null)
                return null;
            matchingCategory.Status = status;
            await _dbContext.SaveChangesAsync();
            return matchingCategory;
        }
    }
}
