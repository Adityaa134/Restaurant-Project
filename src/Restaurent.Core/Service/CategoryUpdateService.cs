using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class CategoryUpdateService : ICategoryUpdateService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryUpdateService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
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
            return updatedCategory.ToCategoryResponse();
        }
    }
}
