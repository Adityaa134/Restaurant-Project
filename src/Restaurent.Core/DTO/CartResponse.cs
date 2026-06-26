using Restaurent.Core.Domain.Entities;

namespace Restaurent.Core.DTO
{
    /// <summary>
    /// Acts as a DTO to return dishes present in the user's cart  
    /// </summary>
    public class CartResponse
    {
        public Guid CartId { get; set; }
        public Guid? UserId { get; set; }
        public Guid DishId { get; set; }
        public int Quantity { get; set; }
        public decimal DishPrice { get; set; }
        public string Dish_Image_Path { get; set; }
        public string DishName { get; set; }
    }

    public static class CartExtensions
    {
        public static CartResponse CartResponse(this Carts cart)
        {
            return new CartResponse
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                DishId = cart.Dishes.DishId,
                Quantity = cart.Quantity,
                DishPrice = cart.Dishes.Price,
                Dish_Image_Path = cart?.Dishes?.Image_Path,
                DishName = cart?.Dishes?.DishName,
            };
        }
    }
}
