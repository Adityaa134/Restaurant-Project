using System;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface ICategoriesGetterService
    {
        /// <summary>
        /// Returns list of all categories from data store
        /// </summary>
        /// <returns>Returns list of all active categories</returns>
        Task<List<CategoryResponse>> GetAllCategories();

        /// <summary>
        /// Returns list of all categories from data store
        /// </summary>
        /// <returns>Returns list of all categories which status is both active or inactive</returns>
        Task<List<CategoryResponse>> GetAllCategoriesAdmin();


        /// <summary>
        /// Search for category 
        /// </summary>
        /// <param name="categoryId">the category to be search</param>
        /// <returns>Returns the category based on id</returns>
        Task<CategoryResponse?> GetCategoryByCategoryId(Guid? categoryId);
    }
}
