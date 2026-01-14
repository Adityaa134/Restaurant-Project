using Microsoft.AspNetCore.Http;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as  a DTO to return dish details 
    /// </summary>
    public class DishResponse
    {
        public Guid? DishId { get; set; }
        public string? DishName { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public IFormFile? Dish_Image { get; set; }

        public string? Dish_Image_Path { get; set; }
    }

    public static class DishExtension
    {
        public static DishResponse ToDishResponse(this Dish dish)
        {
            return new DishResponse()
            {
                DishName = dish.DishName,
                CategoryId = dish.CategoryId,
                DishId = dish.DishId,
                Price = dish.Price,
                Description = dish.Description,
                Dish_Image_Path = dish.Image_Path,
                CategoryName = dish.Category?.CategoryName
            };
        }
    }
}
