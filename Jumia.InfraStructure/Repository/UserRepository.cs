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
    public class UserRepository : Repository<ApplicationUser, string>, IUserRepository
    {

        public UserRepository(JumiaContext context) : base(context)
        {
            }

        // Implement any additional methods specific to ApplicationUser here
    }
}
