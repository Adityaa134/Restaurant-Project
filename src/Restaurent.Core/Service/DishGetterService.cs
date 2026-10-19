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
        private readonly ICacheService _cacheService;

        public DishGetterService(IDishRepository dishRepository, ICategoriesGetterService categoriesGetterService, ICacheService cacheService)
        {
            _dishRepository = dishRepository;
            _categoriesGettterService = categoriesGetterService;
            _cacheService = cacheService;
        }

        public async Task<CursorPaginationResponse<DishResponse>> GetAllDishes(DishPaginationRequest request)
        {
            string version = await _cacheService.GetAsync<string>("dishes:version") ?? "1";
            string cacheKey = $"dishes:v{version}:cursor_{request.CursorDishId ?? Guid.Empty}:take_{request.Take}";

            CursorPaginationResponse<DishResponse>? cachedResponse = await _cacheService.GetAsync<CursorPaginationResponse<DishResponse>>(cacheKey);
            if (cachedResponse != null)
            {
                return cachedResponse;
            }

            var (dishes, hasMore) = await _dishRepository.GetAllDishes(request);
            var last = dishes.Count > 0 ? dishes[^1] : null;

            CursorPaginationResponse<DishResponse> response = new CursorPaginationResponse<DishResponse>
            {
                Items = dishes.Select(x => x.ToDishResponse()).ToList(),
                HasMore = hasMore,
                NextCursorCreatedAt = last?.CreatedAt,
                NextCursorDishId = last?.DishId
            };

            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(15));
            return response;
        }

        public async Task<DishResponse?> GetDishByDishId(Guid? dishId)
        {
            if (dishId == null)
                throw new ArgumentNullException(nameof(dishId));

            string cacheKey = $"dish:{dishId.Value}";

            DishResponse? cachedDish = await _cacheService.GetAsync<DishResponse>(cacheKey);
            if (cachedDish != null)
            {
                return cachedDish;
            }

            Dish? matchingDish = await _dishRepository.GetDishByDishId(dishId.Value);

            if (matchingDish == null)
                return null;

            DishResponse response = matchingDish.ToDishResponse();
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(15));
            return response;
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
