using System.ComponentModel.DataAnnotations;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to update the address of a user
    /// </summary>
    public class AddressUpdateRequest
    {
        [Required(ErrorMessage ="Address Id can't be null")]
        public Guid AddressId { get; set; }
        [Required(ErrorMessage = "User Id can't be null")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "AddressLine can't be null")]
        public string? AddressLine { get; set; }
        [Required(ErrorMessage = "City can't be null")]
        public string? City { get; set; }
        public string? Landmark { get; set; }
        [Required(ErrorMessage = "Area can't be null")]
        public string? Area { get; set; }

        public Address ToAddress()
        {
            return new Address()
            {
                Id = AddressId,
                UserId = UserId,
                AddressLine = AddressLine,
                City = City,
                Landmark = Landmark,
                Area = Area
            };
        }
    }
}
