﻿using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Contract
{

    public interface IAddressRepository : IRepository<Address, int>
    {
        // Define any additional methods specific to the Address entity if needed
    }
}
