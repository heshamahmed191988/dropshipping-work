using Jumia.Application.Contract;
using Jumia.Context;
using Jumia.InfraStructure.Repository;
using Jumia.model;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.InfraStructure
{
    public class CategoryReposatory : Repository<Category, int>, ICategoryReposatory
    {
        public CategoryReposatory(JumiaContext jumiaContext) : base(jumiaContext)
        {

        }
    }
}