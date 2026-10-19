using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class CategoriesGetterService : ICategoriesGetterService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICacheService _cacheService;

        public CategoriesGetterService(ICategoryRepository categoryRepository, ICacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _cacheService = cacheService;
        }

        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            List<CategoryResponse>? cachedCategories = await _cacheService.GetAsync<List<CategoryResponse>>("categories:all");
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            List<Category> categories = await _categoryRepository.GetAllCategories();
            List<CategoryResponse> response = categories.Select(temp => temp.ToCategoryResponse()).ToList();
            await _cacheService.SetAsync("categories:all", response, TimeSpan.FromMinutes(10));

            return response;
        }

        public async Task<List<CategoryResponse>> GetAllCategoriesAdmin()
        {
            List<Category> categories = await _categoryRepository.GetAllCategoriesAdmin();
            return categories.Select(temp => temp.ToCategoryResponse()).ToList();
        }

        public async Task<CategoryResponse?> GetCategoryByCategoryId(Guid? categoryId)
        {
            if(categoryId == null)
                throw new ArgumentNullException(nameof(categoryId));

            string cacheKey = $"category:{categoryId.Value}";

            CategoryResponse? cachedCategory = await _cacheService.GetAsync<CategoryResponse>(cacheKey);
            if (cachedCategory != null)
            {
                return cachedCategory;
            }

            Category? matchingCategory = await _categoryRepository.GetCategoryByCategoryId(categoryId.Value);

            if(matchingCategory == null)
                return null;

            CategoryResponse response = matchingCategory.ToCategoryResponse();
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }

        public async Task<bool> IsCategoryExist(Guid categoryId)
        {
            return await _categoryRepository.IsCategoryExist(categoryId);
        }
    }
}
