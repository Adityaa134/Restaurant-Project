using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface ICategoryUpdateService
    {
        /// <summary>
        /// Update the category status
        /// </summary>
        /// <param name="request">The status deatils to update the category</param>
        /// <returns>Returns the updated details of category</returns>
        Task<CategoryResponse?> UpdateCategoryStatus(CategoryStatusUpdateRequest request);
    }
}
