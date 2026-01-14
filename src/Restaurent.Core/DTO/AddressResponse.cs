using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to return address of user's
    /// </summary>
    public class AddressResponse
    {
        public Guid AddressId { get; set; }
        public Guid? UserId { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Area { get; set; }
        public string? Landmark { get; set; }
    }

    public static class AddressExtension
    {
        public static AddressResponse ToAddressResponse(this Address address)
        {
            return new AddressResponse()
            {
                AddressId = address.Id,
                AddressLine = address.AddressLine,
                Area = address.Area,
                Landmark = address.Landmark,
                UserId = address.UserId,
                City = address.City,
            };
        }
    }

}
