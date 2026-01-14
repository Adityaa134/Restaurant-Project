using System;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.DTO;
namespace Restaurent.Core.Domain.RepositoryContracts
{
    public interface ICategoryRepository
    {

        /// <summary>
        /// Returns list of all categories from data store
        /// </summary>
        /// <returns>Returns list of all active categories</returns>
        Task<List<Category>> GetAllCategories();

        /// <summary>
        /// Returns list of all categories from data store
        /// </summary>
        /// <returns>Returns list of all categories which status is both active or inactive</returns>
        Task<List<Category>> GetAllCategoriesAdmin();


        /// <summary>
        /// Search for category 
        /// </summary>
        /// <param name="categoryId">the category to be search</param>
        /// <returns>Returns the category based on id</returns>
        Task<Category?> GetCategoryByCategoryId(Guid categoryId);

        /// <summary>
        /// Adds a category to the database
        /// </summary>
        /// <param name="category">Contains the category details to add</param>
        /// <returns>Returns the added category details</returns>
        Task<Category> AddCategory(Category? category);

        /// <summary>
        /// Update the category status
        /// </summary>
        /// <param name="status">The new status to update</param>
        /// <param name="categoryId">The category whose status to update</param>
        /// <returns>Returns the updated details of category</returns>
        Task<Category?> UpdateCategoryStatus(bool status, Guid categoryId);
    }
}
