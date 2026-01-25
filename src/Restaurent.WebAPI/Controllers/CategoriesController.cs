using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.WebAPI.Controllers
{
    [Authorize(Roles = "admin")]
    public class CategoriesController : CustomControllerBase
    {

        private readonly ICategoriesGetterService _categoriesGetterService;
        private readonly ICategoryAdderService _categoryAdderService;
        private readonly ICategoryUpdateService _categoryUpdateService;

        public CategoriesController(ICategoriesGetterService categoriesGetterService, ICategoryAdderService categoryAdderService, ICategoryUpdateService categoryUpdateService)
        {
            _categoriesGetterService = categoriesGetterService;
            _categoryAdderService = categoryAdderService;
            _categoryUpdateService = categoryUpdateService;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<ActionResult> GetAllCategories()
        {
            List<CategoryResponse> categories = await _categoriesGetterService.GetAllCategories();
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{categoryId:guid}")]
        public async Task<ActionResult> GetCategoryById(Guid categoryId)
        {
            CategoryResponse? category = await _categoriesGetterService.GetCategoryByCategoryId(categoryId);
            if (category == null)
                return Problem(detail: "Invalid Category Id", statusCode: 400, title: "Category Search");
            return Ok(category);
        }
      
        [HttpPost("add-category")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AddCategory(CategoryAddRequest categoryAddRequest)
        {
           CategoryResponse categoryResponse =  await _categoryAdderService.AddCategory(categoryAddRequest);
            return CreatedAtAction("GetCategoryById", "Categories", new { categoryId = categoryResponse.CategoryId }, categoryResponse);
        }
        
        [HttpPut()]
        public async Task<ActionResult> UpdateCategoryStatus(CategoryStatusUpdateRequest categoryStatusUpdateRequest)
        {
            CategoryResponse? categoryResponse = await _categoryUpdateService.UpdateCategoryStatus(categoryStatusUpdateRequest);
            if (categoryResponse == null)
                return Problem(detail: "Category Status Updation failed", statusCode: 400, title: "Update Category Status");
            return Ok(categoryResponse);
        }

        [HttpGet("admin/categories")]
        public async Task<ActionResult> GetAllCategoriesAdmin()
        {
            List<CategoryResponse> categories = await _categoriesGetterService.GetAllCategoriesAdmin();
            return Ok(categories);
        }
    }
}
