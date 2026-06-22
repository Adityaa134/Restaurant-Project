using Restaurent.Core.Domain.Entities;
using Restaurent.Core.DTO;

namespace Restaurent.Core.ServiceContracts
{
    public interface IRatingsService
    {
        /// <summary>
        /// Adds a new rating to the data store
        /// </summary>
        /// <param name="rating">The rating to add</param>
        /// <returns>Returns the added rating details</returns>
        Task<RatingResponse> AddRating(RatingRequest ratingRequest);

        /// <summary>
        /// Returns all ratings for the given order
        /// </summary>
        /// <param name="orderId">The order to search</param>
        /// <returns>Returns all ratings of the given order</returns>
        Task<List<RatingResponse>?> GetRatingsByOrderId(Guid orderId);
    }
}
