using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customApiApp_3.Controllers
{
    public interface ITApiObjectController<out TData>:ITApiController
    {
        IEnumerable<TData> Get();
        object Get(int id);
    }
}
