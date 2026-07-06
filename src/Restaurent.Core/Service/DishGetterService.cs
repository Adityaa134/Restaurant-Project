using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class DishGetterService : IDishGetterService
    {
        private readonly IDishRepository _dishRepository;
        private readonly ICategoriesGetterService _categoriesGettterService;

        public DishGetterService(IDishRepository dishRepository ,ICategoriesGetterService categoriesGetterService)
        {
            _dishRepository = dishRepository;
            _categoriesGettterService = categoriesGetterService;
        }

        public async Task<CursorPaginationResponse<DishResponse>> GetAllDishes(DishPaginationRequest request)
        {
            var (dishes, hasMore) = await _dishRepository.GetAllDishes(request);
            var last = dishes.Count > 0 ? dishes[^1] : null;

            return new CursorPaginationResponse<DishResponse>
            {
                Items = dishes.Select(x => x.ToDishResponse()).ToList(),
                HasMore = hasMore,
                NextCursorCreatedAt = last?.CreatedAt,
                NextCursorDishId = last?.DishId
            };
        }

        public async Task<DishResponse?> GetDishByDishId(Guid? dishId)
        {
            if(dishId==null)
                throw new ArgumentNullException(nameof(dishId));

            Dish? matchingDish = await _dishRepository.GetDishByDishId(dishId.Value);

            if(matchingDish==null)
                return null;
            return matchingDish.ToDishResponse();
        }

        public async Task<List<DishResponse>?> SearchDish(string searchString)
        {
            if (searchString == null)
                return null;
            List<Dish>? dishes = await _dishRepository.SearchDish(searchString);
            if(dishes == null) return null;
            return dishes.Select(temp=>temp.ToDishResponse())
                                           .ToList();
        }

        public async Task<DishResponse?> ApplyNewRatingToDish(Rating rating)
        {
            if (rating == null)
                throw new ArgumentNullException(nameof(rating));
            bool isDishExist = await _dishRepository.IsDishExist(rating.DishId);
            if (!isDishExist)
                throw new ArgumentException(nameof(rating));
            Dish? updatedDishRating = await _dishRepository.ApplyNewRatingToDish(rating);
            return updatedDishRating?.ToDishResponse();
        }

        public async Task<bool> IsDishExist(Guid dishId)
        {
            return await _dishRepository.IsDishExist(dishId);
        }

        public async Task<List<DishResponse>?> FilterDishes(DishFilterRequest request)
        {
            List<Dish>? dishes = await _dishRepository.FilterDishes(request);
            return dishes?.Select(temp=>temp.ToDishResponse()).ToList();
        }

        public async Task<List<DishResponse>?> GetDishesByCategory(Guid categoryId)
        {
            bool isExist = await _categoriesGettterService.IsCategoryExist(categoryId);
            if (!isExist)
                return null;
            List<Dish> dishes = await _dishRepository.GetDishesByCategory(categoryId);
            return dishes.Select(temp => temp.ToDishResponse()).ToList();
        }
    }
}
