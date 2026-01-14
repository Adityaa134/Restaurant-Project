using System;
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

       
        public async Task<List<DishResponse>> GetAllDishes()
        {

            List<Dish> dishes =  await _dishRepository.GetAllDishes();
            return dishes.Select(temp => temp.ToDishResponse()).ToList();
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

        public async Task<List<DishResponse>?> GetDishesBasedOnCategoryId(Guid? categoryID)
        {
            if(categoryID==null)
                throw new ArgumentNullException(nameof(categoryID));

           CategoryResponse? matchingCategory = await _categoriesGettterService.GetCategoryByCategoryId(categoryID.Value);
          
            if(matchingCategory == null) 
                return null;
            List<Dish> dishes = await _dishRepository.GetDishesBasedOnCategoryId(categoryID.Value);

            return dishes.Select(temp=>temp.ToDishResponse())
                                           .ToList();
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
    }
}
