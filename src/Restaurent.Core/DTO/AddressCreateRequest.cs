using System.ComponentModel.DataAnnotations;
using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to add the address of user
    /// </summary>
    public class AddressCreateRequest
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string? AddressLine { get; set; }
        [Required]
        public string? City { get; set; }
        public string? Landmark { get; set; }
        [Required]
        public string? Area { get; set; }

        public Address ToAddress()
        {
            return new Address()
            {
                UserId = UserId,
                AddressLine = AddressLine,
                City = City,
                Landmark = Landmark,
                Area = Area
            };
        }
    }
}
