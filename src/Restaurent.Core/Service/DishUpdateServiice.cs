using System;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Helpers;
using Restaurent.Core.ServiceContracts;
namespace Restaurent.Core.Service
{
    public class DishUpdateService : IDishUpdateService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IImageUpdateService _imageUpdateService;
        private readonly ICacheService _cacheService;

        public DishUpdateService(IDishRepository dishRepository, IImageUpdateService imageUpdateService, ICacheService cacheService)
        {
            _dishRepository = dishRepository;
            _imageUpdateService = imageUpdateService;
            _cacheService = cacheService;
        }


        public async Task<DishResponse> UpdateDish(DishUpdateRequest? dishUpdateRequest)
        {
            if (dishUpdateRequest == null)
                throw new ArgumentNullException(nameof(dishUpdateRequest));

            if(dishUpdateRequest.DishName == null)
                throw new ArgumentException(nameof(dishUpdateRequest.DishName));

            ValidationHelper.ModelValidator(dishUpdateRequest);

            Dish? matchingDish = await _dishRepository.GetDishByDishId(dishUpdateRequest.DishId.Value);
            if (matchingDish == null)
                throw new ArgumentException("Dish Id does not exist");

            if (dishUpdateRequest.Dish_Image != null && dishUpdateRequest.Dish_Image.Length != 0)
            {
                string? new_Image_Url = await _imageUpdateService.ImageUpdater(dishUpdateRequest.Dish_Image, dishUpdateRequest.Image_Path);

                dishUpdateRequest.Image_Path = new_Image_Url;
            }

            matchingDish.DishName = dishUpdateRequest.DishName;
            matchingDish.Description = dishUpdateRequest.Description;
            matchingDish.Price = dishUpdateRequest.Price.Value;
            matchingDish.Image_Path = dishUpdateRequest.Image_Path;

            Dish updatedDish = await _dishRepository.UpdateDish(matchingDish);
            await _cacheService.RemoveAsync($"dish:{matchingDish.DishId}");
            await _cacheService.SetAsync("dishes:version", DateTime.UtcNow.Ticks.ToString());

            return updatedDish.ToDishResponse();
        }
    }
}
