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
        public DishAdderService(IDishRepository dishRepository, IImageAdderService imageAdderService)
        {
            _dishRepository = dishRepository;
            _imageAdderService = imageAdderService;
        }

        public async Task<DishResponse> AddDish(DishAddRequest? dishAddRequest)
        {
            if(dishAddRequest == null)
                throw new ArgumentNullException(nameof(dishAddRequest));

            if (dishAddRequest.DishName == null || dishAddRequest.Dish_Image == null)
                throw new ArgumentException(nameof(dishAddRequest.DishName),nameof(dishAddRequest.Dish_Image));

            //validations
            ValidationHelper.ModelValidator(dishAddRequest); 

            //Adding image to webroot
            var imgPath =  await _imageAdderService.ImageAdder(dishAddRequest.Dish_Image);
            dishAddRequest.ImagePath_url = imgPath;

            Dish dish = dishAddRequest.ToDish();
            dish.DishId = Guid.NewGuid();

            //calling repository method to add the dish in db
            await _dishRepository.AddDish(dish);

            return dish.ToDishResponse();
        }
    }
}
