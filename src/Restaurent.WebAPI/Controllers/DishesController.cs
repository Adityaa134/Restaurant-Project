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

        public DishesController(IDishGetterService dishGetterService)
        {
            _dishGetterService = dishGetterService;
        }

        [HttpGet()]
        public async Task<ActionResult> GetDishes([FromQuery] DishPaginationRequest request)
        {
            CursorPaginationResponse<DishResponse> dishes  = await _dishGetterService.GetAllDishes(request);
            return Ok(dishes);
        }

        [HttpGet("{dishId:guid}")]
        public async Task<ActionResult> GetDishByDishId(Guid dishId)
        {
           DishResponse? dish =  await _dishGetterService.GetDishByDishId(dishId);
            if (dish == null)
                return Problem(detail: "Dish Not Found", statusCode: StatusCodes.Status404NotFound, title: "Dish Search");

            return Ok(dish);
        }

        [HttpGet("{dish}")]
        public async Task<ActionResult> SearchDish(string dish)
        {
           List<DishResponse>? dishes =  await _dishGetterService.SearchDish(dish);
            return Ok(dishes);
        }

        [HttpGet("filter")]
        public async Task<ActionResult> FilterDishes([FromQuery] DishFilterRequest request)
        {
            List<DishResponse>? dishes =
                await _dishGetterService.FilterDishes(request);
            return Ok(dishes);
        }

        [AllowAnonymous]
        [HttpGet("category/{categoryId:guid}")]
        public async Task<ActionResult> GetDishesByCategory(Guid categoryId)
        {
            List<DishResponse>? dishes = await _dishGetterService.GetDishesByCategory(categoryId);
            if(dishes == null)
                return Problem(detail: "Category Not Found", statusCode: StatusCodes.Status404NotFound, title: "Get Dishes By Category");
            return Ok(dishes);
        }
    }
}
