using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.WebAPI.Controllers
{
    [AllowAnonymous]
    public class DishesController : CustomControllerBase
    {
        private readonly IDishGetterService _dishGetterService;
        private readonly IAddCartItemsService _addCartItemsService;
        private readonly IGetCartItemsService _getCartItemsService;
        private readonly IUpdateItemQuantityInCart _updateItemQuantityInCart;

        public DishesController(IDishGetterService dishGetterService,IAddCartItemsService addCartItemsService,IGetCartItemsService getCartItemsService,IUpdateItemQuantityInCart updateItemQuantityInCart)
        {
            _dishGetterService = dishGetterService;
            _addCartItemsService = addCartItemsService;
            _getCartItemsService = getCartItemsService;
            _updateItemQuantityInCart = updateItemQuantityInCart;
        }

        [HttpGet()]
        public async Task<ActionResult> GetDishes()
        {
            List<DishResponse> dishes  = await _dishGetterService.GetAllDishes();
            return Ok(dishes);
        }

        [HttpGet("{dishId:guid}")]
        public async Task<ActionResult> GetDishByDishId(Guid dishId)
        {
           DishResponse? dish =  await _dishGetterService.GetDishByDishId(dishId);
            if (dish == null)
                return Problem(detail: "Invalid Dish Id", statusCode: 400, title: "Dish Search");

            return Ok(dish);
        }

        [HttpPost("add-to-cart")]
        public async Task<ActionResult> AddToCart(AddToCartRequest addToCartRequest)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

           AddToCartResponse addToCartResponse =   await _addCartItemsService.AddItemToCart(addToCartRequest);
            return Ok(addToCartResponse);
        }

        [HttpGet("GetCartItems")]
        public async Task<ActionResult> GetCartItems([FromQuery] Guid? userId)
        
        {
          List<AddToCartResponse> cartItems =  await _getCartItemsService.GetAllCartItems(userId);
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
           bool exist =  await _getCartItemsService.IsCartItemExist(userId, dishId);
            if (exist)
                return Ok(true);
            return Ok(false);
        }

        [HttpGet("{dish}")]
        public async Task<ActionResult> SearchDish(string dish)
        {
           List<DishResponse>? dishes =  await _dishGetterService.SearchDish(dish);
            return Ok(dishes);
        }
    }
}
