using Jumia.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jumia.Mvc.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentServices _paymentServices;

        public PaymentController(IPaymentServices paymentServices)
        {
            _paymentServices = paymentServices;
        }

        public async Task<IActionResult> Index()
        {
            var payments = await _paymentServices.GetAllPaymentsAsync();
            return View(payments);
        }
    }
}
