using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface ICategoryAdderService
    {
        /// <summary>
        /// Adds a category to the database
        /// </summary>
        /// <param name="categoryAddRequest">Contains the category details to add</param>
        /// <returns>Returns the added category details</returns>
        Task<CategoryResponse> AddCategory(CategoryAddRequest? categoryAddRequest);
    }
}
