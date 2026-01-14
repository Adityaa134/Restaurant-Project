using System.ComponentModel.DataAnnotations;
using Restaurent.Core.Domain.Identity;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to register a user
    /// </summary>
    public class RegisterRequestt
    {
        [Required(ErrorMessage = "UserName is required")]
        [Length(5, 10, ErrorMessage = "User Name should be between 5 to 10 characters")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "UserName should only contains digits , alphabets and underscore")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email should be in a proper fromat")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone Number should contains only digits")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public ApplicationUser ToApplicationUser()
        {
            return new ApplicationUser()
            {
                UserName = UserName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                PasswordHash = Password
            };
        }
    }
}

     
    
