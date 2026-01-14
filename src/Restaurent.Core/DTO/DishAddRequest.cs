using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Restaurent.Core.CustomValidators;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to add a Dish 
    /// </summary>
    public class DishAddRequest
    {
        [Required(ErrorMessage = "Dish Name can't be blank")]
        [StringLength(40)]
        public string? DishName { get; set; }

        [Required(ErrorMessage = "Dish Price can't be blank")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Dish Description is required")]
        [StringLength(600)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please Select a category of dish")]
        public string? CategoryId { get; set; }

        [Required(ErrorMessage = "Dish image  is required")]
        [ValidatingFileType]
        public IFormFile? Dish_Image { get; set; }

        public string? ImagePath_url { get; set; }

        public Dish ToDish()
        {
            return new Dish()
            {
                Price = Price.Value,
                CategoryId = Guid.Parse(CategoryId),
                Description = Description,
                DishName = DishName,
                Image_Path = ImagePath_url
            };
        }
    }
}
