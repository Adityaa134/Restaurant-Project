using System;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class CategoriesGetterService : ICategoriesGetterService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesGetterService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }


        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            List<Category> categories = await _categoryRepository.GetAllCategories();
            return categories.Select(temp => temp.ToCategoryResponse()).ToList();
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

            Category? matchingCategory = await _categoryRepository.GetCategoryByCategoryId(categoryId.Value);

            if(matchingCategory == null)
                return null;
            return matchingCategory.ToCategoryResponse();
        }
    }
}
