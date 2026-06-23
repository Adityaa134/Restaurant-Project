using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Helpers;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class RatingsService : IRatingsService
    {
        private readonly IRatingsRepository _ratingsRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IDishGetterService _dishGetterService;
        public RatingsService(IRatingsRepository ratingsRepository, IOrdersRepository ordersRepository, IDishGetterService dishGetterService)
        {
            _ratingsRepository = ratingsRepository;
            _ordersRepository = ordersRepository;
            _dishGetterService = dishGetterService;
        }

        public async Task<RatingResponse> AddRating(RatingRequest ratingRequest)
        {
            if(ratingRequest == null) 
                throw new ArgumentNullException(nameof(ratingRequest));
            if (ratingRequest.Rate < 1 || ratingRequest.Rate > 5)
                throw new ArgumentException("Rating can be given between 1 and 5");

            ValidationHelper.ModelValidator(ratingRequest);

            bool isOrderOwnedByUser = await _ordersRepository.IsOrderOwnedByUser(ratingRequest.UserId, ratingRequest.OrderId);
            if (!isOrderOwnedByUser)
                throw new ArgumentException("Order is not valid");

            bool isDishInOrder =
            await _ordersRepository.IsDishPartOfOrder(ratingRequest.OrderId, ratingRequest.DishId);

            if (!isDishInOrder)
                throw new InvalidOperationException("Dish is not present in order");

            bool isRatingExist = await _ratingsRepository.IsRatingGiven(ratingRequest.OrderId,ratingRequest.DishId);

            if (isRatingExist)
                throw new InvalidOperationException("Rating is alerady given.");


            Rating rating = ratingRequest.ToRating();
            rating.Id = Guid.NewGuid();
            await _ratingsRepository.AddRating(rating);
            await _dishGetterService.ApplyNewRatingToDish(rating);
            return rating.ToRatingResponse();
        }
        public async Task<List<RatingResponse>?> GetRatingsByOrderId(Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentNullException(nameof(orderId));

            List<Rating>? ratings =
                await _ratingsRepository.GetRatingsByOrderId(orderId);

            if (ratings == null)
                return null;

            return ratings
                .Select(r => r.ToRatingResponse())
                .ToList();
        }
    }
}
