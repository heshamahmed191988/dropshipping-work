using Microsoft.AspNetCore.Mvc;
using Jumia.Application.Contract;
using Jumia.Dtos;
using System.Threading.Tasks;
using Jumia.Application.Services;
using Jumia.Model;

namespace Jumia.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentServices _paymentService;

        public PaymentController(IPaymentServices paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/Payment
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment(int orderId)
        {
            try
            {
                var paymentDto = await _paymentService.CreatePaymentAsync(orderId);
                return Ok(paymentDto);
            }
            catch (KeyNotFoundException knfe)
            {
                return NotFound(knfe.Message);
            }
            catch (System.Exception ex)
            {
                // Generic exception handling, consider logging the exception details
                return StatusCode(500, "An error occurred while creating the payment. Please try again later.");
            }
        }
    }
}