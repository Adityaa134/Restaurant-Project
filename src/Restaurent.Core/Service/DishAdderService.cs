using System;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.DTO;
using Restaurent.Core.Helpers;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class DishAdderService : IDishAdderService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IImageAdderService _imageAdderService;
        private readonly ICacheService _cacheService;

        public DishAdderService(IDishRepository dishRepository, IImageAdderService imageAdderService, ICacheService cacheService)
        {
            _dishRepository = dishRepository;
            _imageAdderService = imageAdderService;
            _cacheService = cacheService;
        }

        public async Task<DishResponse> AddDish(DishAddRequest? dishAddRequest)
        {
            if(dishAddRequest == null)
                throw new ArgumentNullException(nameof(dishAddRequest));

            if (dishAddRequest.DishName == null || dishAddRequest.Dish_Image == null)
                throw new ArgumentException(nameof(dishAddRequest.DishName),nameof(dishAddRequest.Dish_Image));

            ValidationHelper.ModelValidator(dishAddRequest); 
            
            var imgPath =  await _imageAdderService.ImageAdder(dishAddRequest.Dish_Image);
            dishAddRequest.ImagePath_url = imgPath;

            Dish dish = dishAddRequest.ToDish();
            dish.DishId = Guid.NewGuid();
            
            await _dishRepository.AddDish(dish);
            await _cacheService.SetAsync("dishes:version", DateTime.UtcNow.Ticks.ToString());

            return dish.ToDishResponse();
        }
    }
}
