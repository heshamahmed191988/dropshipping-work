using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Contract
{
    public interface IUserRepository : IRepository<ApplicationUser, string>
    {
        // Define additional methods specific to the ApplicationUser entity if necessary
    }
}
