using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Restaurent.Core.CustomValidators;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Represents user personal details used for adding or updating profile information.
    /// </summary>
    public class UserPersonalDetailsDTO
    {
        [Required(ErrorMessage = "User Id is required")]
        public Guid? UserId { get; set; }

        [ValidatingFileType(isRequiredForNew: false)]
        public IFormFile? ProfileImage { get; set; }
        public string? ProfileImagePath { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        [Length(5, 10, ErrorMessage = "User Name should be between 5 to 10 characters")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "UserName should only contains digits , alphabets and underscore")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone Number should contains only digits")]
        public string? PhoneNumber { get; set; }
    }
}
