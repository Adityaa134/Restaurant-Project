using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class CategoryUpdateService : ICategoryUpdateService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICacheService _cacheService;

        public CategoryUpdateService(ICategoryRepository categoryRepository, ICacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _cacheService = cacheService;
        }

        public async Task<CategoryResponse?> UpdateCategoryStatus(CategoryStatusUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            var category = await _categoryRepository.GetCategoryByCategoryId(request.CategoryId);
            if (category == null)
                throw new ArgumentException("Invalid Category Id");
            Category? updatedCategory = await _categoryRepository.UpdateCategoryStatus(request.Status, request.CategoryId);
            if (updatedCategory == null)
                return null;

            await _cacheService.RemoveAsync($"category:{request.CategoryId}");
            await _cacheService.RemoveAsync("categories:all");

            return updatedCategory.ToCategoryResponse();
        }
    }
}
