using Jumia.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public interface IAddressService
    {
        Task<AddressDto> CreateAddressAsync(AddressDto addressDto);
        Task<AddressDto> GetAddressByIdAsync(int id);
        // Add other methods as needed
    }
}
