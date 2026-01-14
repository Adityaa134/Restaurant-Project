using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Restaurent.Core.CustomValidators;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to add a category 
    /// </summary>
    public class CategoryAddRequest
    {
        [Required(ErrorMessage = "Category Name can't be blank")]
        [StringLength(200)]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Status can't be blank")]
        public bool Status { get; set; }

        [Required(ErrorMessage = "Category image  is required")]
        [ValidatingFileType]
        public IFormFile? Cat_Image { get; set; }
        public string? Cat_Image_Path { get; set; }

        public Category ToCategory()
        {
            return new Category()
            {
                CategoryName = CategoryName,
                Status = Status,
                Cat_Image = Cat_Image_Path
            };
        }
    }
   
}

