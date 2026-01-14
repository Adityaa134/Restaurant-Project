using System.ComponentModel.DataAnnotations;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to login the user  
    /// </summary>
    public class LoginRequestt
    {
        [Required(ErrorMessage = "UserName is required")]
        [Length(5, 10, ErrorMessage = "User Name should be between 5 to 10 characters")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "UserName should only contains digits , alphabets and underscore")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
