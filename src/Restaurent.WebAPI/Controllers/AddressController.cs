using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurent.Core.DTO;
using Restaurent.Core.ServiceContracts;

namespace Restaurent.WebAPI.Controllers
{
    [Authorize]
    public class AddressController : CustomControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        [HttpPost()]
        public async Task<ActionResult> CreateAddress(AddressCreateRequest addressCreateRequest)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

            AddressResponse addressResponse = await _addressService.CreateAddress(addressCreateRequest);

            if (addressResponse == null)
            {
                return Problem("Error in creating address");
            }
            return CreatedAtAction("GetAddressById", "Address", new { addressId = addressResponse.AddressId }, addressResponse);
        }

        [HttpPut()]
        public async Task<ActionResult> UpdateAddress(AddressUpdateRequest addressUpdateRequest)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join("|", ModelState.Values.SelectMany(value => value.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }
            AddressResponse addressResponse = await _addressService.UpdateAddress(addressUpdateRequest);
            if (addressResponse == null)
                return Problem("Error in updating address");
            return Ok(addressResponse);
        }

        [HttpGet("user-address/{userId:guid}")]
        public async Task<ActionResult> GetAddressesOfUser(Guid userId)
        {
            List<AddressResponse>? addressResponses = await _addressService.GetAddressesByUserId(userId);
            return Ok(addressResponses);
        }
        [HttpGet("{addressId:guid}")]
        public async Task<ActionResult> GetAddressById(Guid addressId)
        {
           AddressResponse? addressResponse =  await _addressService.GetAddressByAddressId(addressId);
            return Ok(addressResponse);
        }
    }
}
