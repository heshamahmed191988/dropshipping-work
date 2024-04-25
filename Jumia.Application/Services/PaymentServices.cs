using AutoMapper;
using Jumia.Application.Contract;
using Jumia.Dtos;
using Jumia.Dtos.ResultView;
using Jumia.Dtos.ViewModel.category;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IPaymentReposatory _paymentRepository;
        private readonly IMapper _mapper;
        private readonly IOrderReposatory _orderRepository;

        public PaymentServices(IPaymentReposatory paymentRepository, IMapper mapper, IOrderReposatory orderRepository)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<PaymentDto> CreatePaymentAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            var payment = new Payment
            {
                orderID = orderId,
                DatePaid = DateTime.Now,
                paymentMethod = "paypal"
            };

            var createdPayment = await _paymentRepository.CreateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(createdPayment);
        }
        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return _mapper.Map<List<PaymentDto>>(payments);
        }

    }
}