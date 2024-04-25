using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.model
{
    public interface IBaseEntity
    {
        bool IsDeleted { get; set; }
    }
}
