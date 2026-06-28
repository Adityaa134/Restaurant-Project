using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    public class ReorderCartItemRequest
    {
        public Guid DishId { get; set; }
        public int Quantity { get; set; }
        public Guid UserId { get; set; }

        public Carts ToCart()
        {
            return new Carts()
            {
                UserId = UserId,
                DishId = DishId,
                Quantity = Quantity
            };
        }
    }
}
