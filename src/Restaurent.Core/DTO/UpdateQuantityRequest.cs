using System.ComponentModel.DataAnnotations;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Represents a request to update the quantity of an item in the cart.
    /// </summary>
    public class UpdateQuantityRequest
    {
        public int Quantity { get; set; }
        [Required]
        public Guid CartId { get; set; }
    }
}
