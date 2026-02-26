using System;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.Domain.RepositoryContracts;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.Core.Service
{
    public class DishDeleteService : IDishDeleteService
    {
        private readonly IDishRepository _dishRepository;
        private readonly IImageDeleteService _imageDeleteService;

        public  DishDeleteService(IDishRepository dishRepository, IImageDeleteService imageDeleteService)
        {
            _dishRepository = dishRepository;  
            _imageDeleteService = imageDeleteService;
        }
        public async Task<bool> DeleteDish(Guid? dishId)
        {
            if(dishId == null)
                throw new ArgumentNullException(nameof(dishId));

            Dish? matchingDish = await _dishRepository.GetDishByDishId(dishId.Value);

            if (matchingDish == null)
                return false;

            bool isImageDeleted = await _imageDeleteService.ImageDeleter(matchingDish.Image_Path);

            if(!isImageDeleted) 
                return false;

            bool isDishDeleted = await _dishRepository.DeleteDishByDishId(dishId.Value);
            if(isDishDeleted == false) 
                return false;
            return true;
        }
    }
}
