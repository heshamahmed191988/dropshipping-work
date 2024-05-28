using Jumia.Application.Contract;
using Jumia.Dtos;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IOrderReposatory orderReposatory;

        public AddressService(IAddressRepository addressRepository,IOrderReposatory  orderReposatory)
        {
            _addressRepository = addressRepository;
            this.orderReposatory = orderReposatory;
        }

        public async Task<AddressDto> CreateAddressAsync(AddressDto addressDto)
        {
            var address = new Address // Map DTO to Address entity
            {
                Street = addressDto.Street,
                City = addressDto.City,
                State = addressDto.State,
                ZipCode = addressDto.ZipCode,
                UserID = addressDto.UserID,
                clientName=addressDto.clientName
            };

            // Assuming an Order exists and we are associating it directly
            // This part is highly dependent on your specific business logic
            foreach (var orderId in addressDto.OrderIds)
            {
                var order = await orderReposatory.GetByIdAsync(orderId); // You need an order repository
                if (order != null)
                {
                    // This assumes Address entity has a way to link to orders, like an Orders collection
                    address.Orders.Add(order);
                }
            }

            await _addressRepository.CreateAsync(address);
            await _addressRepository.SaveChangesAsync();
            addressDto.Id = address.Id; // Reflect the ID assigned by the database
            return addressDto;
        }

        //public async Task<AddressDto> CreateAddressAsync(AddressDto addressDto)
        //{
        //    var address = new Address // Map DTO to Address entity
        //    {
        //        Street = addressDto.Street,
        //        City = addressDto.City,
        //        State = addressDto.State,
        //        ZipCode = addressDto.ZipCode,
        //        UserID = addressDto.UserID
        //    };

        //    await _addressRepository.CreateAsync(address);
        //    await _addressRepository.SaveChangesAsync();
        //    addressDto.Id = address.Id; // Reflect the ID assigned by the database
        //    return addressDto;
        //}

        public async Task<AddressDto> GetAddressByIdAsync(int id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null) return null;

            // Assuming your Address entity includes a navigation property to Orders
            var orderIds = address.Orders?.Select(order => order.Id).ToList();

            return new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                UserID = address.UserID,
                OrderIds = orderIds ?? new List<int>() // Include the order IDs
            };
        }
    }
}
