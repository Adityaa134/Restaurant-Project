using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Helpers;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class CategoryAdderService : ICategoryAdderService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageAdderService _imageAdderService;

        public CategoryAdderService(ICategoryRepository categoryRepository, IImageAdderService imageAdderService)
        {
            _categoryRepository = categoryRepository;
            _imageAdderService = imageAdderService;
        }

        public async Task<CategoryResponse> AddCategory(CategoryAddRequest? categoryAddRequest)
        {
            if(categoryAddRequest == null)
                throw new ArgumentNullException(nameof(categoryAddRequest));

            ValidationHelper.ModelValidator(categoryAddRequest);

            if (categoryAddRequest.CategoryName == null || categoryAddRequest.Cat_Image == null)
                throw new ArgumentException(nameof(categoryAddRequest.CategoryName), nameof(categoryAddRequest.Cat_Image));

            var imagePath = await _imageAdderService.ImageAdder(categoryAddRequest.Cat_Image);
            categoryAddRequest.Cat_Image_Path = imagePath;

            Category category = categoryAddRequest.ToCategory();
            category.Id = Guid.NewGuid();

            await _categoryRepository.AddCategory(category);
            return category.ToCategoryResponse();
        }
    }
}
