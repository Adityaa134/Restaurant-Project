using System;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IDishGetterService
    {
        /// <summary>
        /// Returns all dishes from the data store
        /// </summary>
        /// <returns>Returns all dishes</returns>
        Task<List<DishResponse>> GetAllDishes();


        /// <summary>
        /// Search for dish based on id
        /// </summary>
        /// <param name="dishId">the dish to be search</param>
        /// <returns>Retuns dish based on id</returns>
        Task<DishResponse?> GetDishByDishId(Guid? dishId);


        /// <summary>
        ///  Gives all dishes based on categoryId from the database
        /// </summary>
        /// <param name="categoryID">CategoryId baesd on which dishes will be returned</param>
        /// <returns>Returns all the dishes based on  category id</returns>
        Task<List<DishResponse>?> GetDishesBasedOnCategoryId(Guid? categoryID);

        /// <summary>
        /// Returns all dishes which matches with given searchString
        /// </summary>
        /// <param name="searchString">search String to search</param>
        /// <returns>Returns all matching dishes based on searchString</returns>
        Task<List<DishResponse>?> SearchDish(string searchString);

        /// <summary>
        /// Checks if a dish exist
        /// </summary>
        /// <param name="dishId">The dish to check</param>
        /// <returns>Returns true if exist otherwise false</returns>
        Task<bool> IsDishExist(Guid dishId);

        /// <summary>
        /// Applies the new rating to the dish and updates its average rating and total ratings
        /// </summary>
        /// <param name="rating">The rating details to apply</param>
        /// <returns>Returns the updated dish details</returns>
        Task<DishResponse?> ApplyNewRatingToDish(Rating rating);

        /// <summary>
        /// Returns dishes based on the applied price and rating filters
        /// </summary>
        /// <param name="request">The filter criteria for dishes</param>
        /// <returns>Returns all matching dishes based on filter conditions</returns>
        Task<List<DishResponse>?> FilterDishes(DishFilterRequest request);
    }
}
