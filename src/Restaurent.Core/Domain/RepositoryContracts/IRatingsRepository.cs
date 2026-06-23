using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.Domain.RepositoryContracts
{
    public interface IRatingsRepository
    {
        /// <summary>
        /// Adds a new rating to the data store
        /// </summary>
        /// <param name="rating">The rating to add</param>
        /// <returns>Returns the added rating details</returns>
        Task<Rating> AddRating(Rating rating);

        /// <summary>
        /// Returns all ratings for the given order
        /// </summary>
        /// <param name="orderId">The order to search</param>
        /// <returns>Returns all ratings of the given order</returns>
        Task<List<Rating>?> GetRatingsByOrderId(Guid orderId);

        /// <summary>
        /// Checks whether rating is already given for the dish in the order
        /// </summary>
        /// <param name="orderId">The order to check</param>
        /// <param name="dishId">The dish to check</param>
        /// <returns>Returns true if rating exists otherwise false</returns>
        Task<bool> IsRatingGiven(Guid orderId, Guid dishId);
    }
}
