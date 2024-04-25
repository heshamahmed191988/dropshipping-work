using Jumia.Application.Services;
using Jumia.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmazonWebSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // GET: api/Address/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDto>> GetAddress(int id)
        {
            var addressDto = await _addressService.GetAddressByIdAsync(id);
            if (addressDto == null)
            {
                return NotFound();
            }

            return Ok(addressDto);
        }

        // POST: api/Address
        [HttpPost]
        public async Task<ActionResult<AddressDto>> PostAddress([FromBody] AddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAddressDto = await _addressService.CreateAddressAsync(addressDto);
            return CreatedAtAction(nameof(GetAddress), new { id = createdAddressDto.Id }, createdAddressDto);
        }
    }
}
