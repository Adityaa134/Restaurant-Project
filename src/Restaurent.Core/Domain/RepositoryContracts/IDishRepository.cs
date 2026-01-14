using System;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.Domain.RepositoryContracts
{
    public interface IDishRepository
    {
        /// <summary>
        /// Add Dish in the data store
        /// </summary>
        /// <param name="dish">dish to add in data store</param>
        /// <returns>Returns the added dish details</returns>
        Task<Dish> AddDish(Dish dish);


        /// <summary>
        /// Updates the dish in the data store
        /// </summary>
        /// <param name="dish">dish to update</param>
        /// <returns>Returns the updated dish details</returns>
        Task<Dish> UpdateDish(Dish dish);


        /// <summary>
        /// Delete the dish from the data store
        /// </summary>
        /// <param name="dishId">The dish to delete</param>
        /// <returns>Returns true if dish is deleted ; otherwise false</returns>
        Task<bool> DeleteDishByDishId(Guid dishId);


        /// <summary>
        /// Returns all dishes from the data store
        /// </summary>
        /// <returns>Returns all dishes</returns>
        Task<List<Dish>> GetAllDishes();


        /// <summary>
        /// Search for the dish based on the dish id 
        /// </summary>
        /// <param name="dishId">the dish id to search</param>
        /// <returns>Returns the dish from the data store</returns>
        Task<Dish?> GetDishByDishId(Guid dishId);


        /// <summary>
        /// Returns All dishes based on category id
        /// </summary>
        /// <param name="categoryId">the id based on which the dishes will be returned </param>
        /// <returns>Returns the list of dishes based on category id</returns>
        Task<List<Dish>> GetDishesBasedOnCategoryId(Guid categoryId);

        /// <summary>
        /// Returns all dishes which matches with given  searchString
        /// </summary>
        /// <param name="searchString">search String to search</param>
        /// <returns>Returns all matching dishes based on searchString</returns>
        Task<List<Dish>?> SearchDish(string searchString);
    }
}
