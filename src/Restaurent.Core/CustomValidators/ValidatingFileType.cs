using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Restaurent.Core.CustomValidators
{
    public class ValidatingFileTypeAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly int _maxFileSizeMb = 5;
        private readonly bool _isRequiredForNew;

        public ValidatingFileTypeAttribute(bool isRequiredForNew = true)
        {
            _isRequiredForNew = isRequiredForNew;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            // Checking if fille is null
            if (file == null || file.Length == 0)
            {

                if (IsNewProduct(validationContext) && _isRequiredForNew)
                {
                    return new ValidationResult("Dish image is required for new products");
                }
                return ValidationResult.Success;
            }

            // if file is not null means we are validating it (it can be of time adding a dish or updating a dish)
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                return new ValidationResult("Only .jpg, .jpeg, .png types are allowed");
            }

            if (file.Length > _maxFileSizeMb * 1024 * 1024)
            {
                return new ValidationResult($"File size must be less than {_maxFileSizeMb} MB");
            }

            return ValidationResult.Success;
        }

        private bool IsNewProduct(ValidationContext context)
        {
            //Checking if dishid is exist or not and getting that dishid value by reflection
            var idProperty = context.ObjectType.GetProperty("DishId");
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(context.ObjectInstance);
                return idValue == null || idValue.Equals(0);
            }
            return true; 
        }
    }
}
