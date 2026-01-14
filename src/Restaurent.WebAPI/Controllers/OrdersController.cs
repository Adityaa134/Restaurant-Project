using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurent.Core.DTO;
using Restaurent.Core.Enums;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.WebAPI.Controllers
{
    [Authorize]
    public class OrdersController : CustomControllerBase
    {
        private readonly IOrderCreateService _orderCreateService;
        private readonly IOrderGetterService _orderGetterService;
        private readonly IOrderUpdateService _orderUpdateService;

        public OrdersController(IOrderCreateService orderCreateService, IOrderGetterService orderGetterService, IOrderUpdateService orderUpdateService)
        {
            _orderCreateService = orderCreateService;
            _orderGetterService = orderGetterService;
            _orderUpdateService = orderUpdateService;
        }

        [HttpGet()]
        public async Task<ActionResult> GetOrders([FromQuery] PaginationRequest request)
        {
            PaginationResponse<OrderResponse>? orderResponse =  await _orderGetterService.GetOrders(request);
            return Ok(orderResponse);
        }

        [HttpPost()]
        public async Task<ActionResult> CreateOrder(OrderAddRequest orderAddRequest)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

            OrderResponse orderResponse  = await _orderCreateService.CreateOrder(orderAddRequest);
            if (orderResponse == null)
            {
                return Problem("Error in creating order");
            }

            return CreatedAtAction("GetOrderByOrderId", "Orders", new { orderId = orderResponse.OrderId }, orderResponse);
        }

        [HttpGet("user-orders/{userId:guid}")]
        public async Task<ActionResult> GetOrdersByUserId(Guid userId)
        {
            List<OrderResponse>? orderResponses =  await _orderGetterService.GetOrdersByUserId(userId);
            return Ok(orderResponses);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult> GetOrderByOrderId(Guid orderId)
        {
           OrderResponse? orderResponse =  await _orderGetterService.GetOrderByOrderId(orderId);
            if (orderResponse == null)
                return Problem(detail: "Invalid Order Id", statusCode: 400, title: "Order Search");
            return Ok(orderResponse);
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<ActionResult> UpdateOrderStatus(UpdateOrderStatusRequest request) 
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

            OrderResponse? orderResponse = await _orderUpdateService.UpdateOrderStatus(request);
            if(orderResponse == null)
                return Problem(detail:"Order Status Updation failed", statusCode: 400, title: "Update Orders Status");
            return Ok(orderResponse);
        }

        [HttpPut("cancel-order")]
        public async Task<IActionResult> CancelOrder(UpdateOrderStatusRequest request)
        {
            if (request.OrderStatus != OrderStatus.Cancelled)
                return BadRequest("Only cancellation is allowed");

            var result = await _orderUpdateService.UpdateOrderStatusToCancel(request);
            return Ok(result);
        }
    }
}
