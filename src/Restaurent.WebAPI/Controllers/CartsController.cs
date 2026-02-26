using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.WebAPI.Controllers
{
    [AllowAnonymous]
    public class CartsController : CustomControllerBase
    {
        private readonly IAddCartItemsService _addCartItemsService;
        private readonly IGetCartItemsService _getCartItemsService;
        private readonly IUpdateItemQuantityInCart _updateItemQuantityInCart;

        public CartsController(IAddCartItemsService addCartItemsService, IGetCartItemsService getCartItemsService, IUpdateItemQuantityInCart updateItemQuantityInCart)
        {
            _addCartItemsService = addCartItemsService;
            _getCartItemsService = getCartItemsService;
            _updateItemQuantityInCart = updateItemQuantityInCart;
        }

        [HttpPost("add-to-cart")]
        public async Task<ActionResult> AddToCart(AddToCartRequest addToCartRequest)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return ValidationProblem(detail: errorMessage,
                    statusCode: StatusCodes.Status400BadRequest);
            }

            AddToCartResponse addToCartResponse = await _addCartItemsService.AddItemToCart(addToCartRequest);
            return Ok(addToCartResponse);
        }

        [HttpGet("GetCartItems")]
        public async Task<ActionResult> GetCartItems([FromQuery] Guid? userId)
        {
            List<AddToCartResponse> cartItems = await _getCartItemsService.GetAllCartItems(userId);
            return Ok(cartItems);
        }

        [HttpPut("update-quantity")]
        public async Task<ActionResult> UpdateQuantity(UpdateQuantityRequest updateQuantityRequest)
        {
            var updatedCart = await _updateItemQuantityInCart.UpdateDishQuantityInCartItem(updateQuantityRequest);
            return Ok(updatedCart);
        }

        [HttpGet("CheckCartItemExist")]
        public async Task<ActionResult> CheckCartItemExist([FromQuery] Guid? userId, [FromQuery] Guid dishId)
        {
            bool exist = await _getCartItemsService.IsCartItemExist(userId, dishId);
            if (exist)
                return Ok(true);
            return Ok(false);
        }
    }
}
