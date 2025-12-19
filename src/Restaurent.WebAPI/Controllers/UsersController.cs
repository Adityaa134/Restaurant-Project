using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Restaurent.Core.Domain.Entities;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.WebAPI.Controllers
{
    [Authorize]
    public class UsersController : CustomControllerBase
    {
        private readonly IAuthService _authService;
        public UsersController(IAuthService authService)
        { 
            _authService = authService;
        }

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult> GetUserById(Guid userId)
        {
            UserDTO? userDTO =  await _authService.GetUserByUserId(userId);
            if (userDTO == null)
                return Problem(detail: "Invalid User Id", statusCode: 400, title: "User Search");
            return Ok(userDTO);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AddProfileImage(UserPersonalDetailsDTO personalDetailsDTO)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }
            UserDTO? userDTO = await _authService.UpdatePersonalDetails(personalDetailsDTO);
            if (userDTO == null)
                return BadRequest();
            return Ok(userDTO);
        }
    }
}
