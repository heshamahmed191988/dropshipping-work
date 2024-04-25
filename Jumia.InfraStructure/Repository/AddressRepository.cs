using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.InfraStructure.Repository
{
    public class AddressRepository : Repository<Address, int>, IAddressRepository
    {
        public AddressRepository(JumiaContext context) : base(context)
        {
        }

        // Implement any additional methods specific to the Address entity
    }
}
