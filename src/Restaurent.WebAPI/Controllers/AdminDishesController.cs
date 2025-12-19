using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.WebAPI.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminDishesController : CustomControllerBase
    {
        private readonly IDishAdderService _dishAdderService;
        private readonly IDishDeleteService _dishDeleteService;
        private readonly IDishUpdateService _dishUpdateService;

        public AdminDishesController(IDishAdderService dishAdderService, IDishDeleteService dishDeleteService, IDishUpdateService dishUpdateService)
        {
            _dishAdderService = dishAdderService;
            _dishDeleteService = dishDeleteService;
            _dishUpdateService = dishUpdateService;
        }

        [HttpPost("add-dish")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AddDish([FromForm] DishAddRequest dishAddRequest)
        {
            DishResponse dishResponse = await _dishAdderService.AddDish(dishAddRequest);
            return CreatedAtAction("GetDishByDishId","Dishes", new { dishId = dishResponse.DishId }, dishResponse); //status code 201(means created)
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> PutDish(DishUpdateRequest dishUpdateRequest)
        {
           var dishUpdated =  await _dishUpdateService.UpdateDish(dishUpdateRequest);
            return Ok(dishUpdated);
        }

        [HttpDelete("{dishId:guid}")]
        public async Task<ActionResult> DeleteDish(Guid dishId)
        {
            bool isDishDeleted = await _dishDeleteService.DeleteDish(dishId);
            if (!isDishDeleted)
                return Problem(detail: "Invalid DishId", statusCode: 400, title: "Delete Dish");
            return NoContent();
        }

    }
}
