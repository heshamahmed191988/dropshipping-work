﻿using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Contract
{
    public interface IOrderProuduct : IRepository<OrderProduct, int>
    {
        Task<OrderProduct> Sreach(int OrderID);
    }
}