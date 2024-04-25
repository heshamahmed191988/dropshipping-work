using Jumia.Dtos;
using Jumia.Dtos.ResultView;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public interface IPaymentServices
    {
        Task<PaymentDto> CreatePaymentAsync(int orderId);
        Task<List<PaymentDto>> GetAllPaymentsAsync();

    }
}
