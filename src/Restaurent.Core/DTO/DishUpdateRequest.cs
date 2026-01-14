using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Restaurent.Core.CustomValidators;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to update a Dish 
    /// </summary>
    public class DishUpdateRequest
    {
        [Required(ErrorMessage = "Dish Id is required")]
        public Guid? DishId { get; set; }

        [Required(ErrorMessage = "Dish Name can't be blank")]
        [StringLength(40)]
        public string? DishName { get; set; }

        [Required(ErrorMessage = "Dish Price can't be blank")]
        [DataType(DataType.Currency)]
        public int? Price { get; set; }

        [Required(ErrorMessage = "Dish Description is required")]
        [StringLength(600)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please Select a category of dish")]
        public Guid? CategoryId { get; set; }


        [ValidatingFileType(isRequiredForNew: false)]
        public IFormFile? Dish_Image { get; set; }

        public string? Image_Path { get; set; }

        public Dish ToDish()
        {
            return new Dish()
            {
                Price = Price.Value,
                CategoryId = CategoryId.Value,
                Description = Description,
                DishName = DishName,
                Image_Path = Image_Path
            };
        }
    }
}
