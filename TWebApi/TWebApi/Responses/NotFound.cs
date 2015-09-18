using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customApiApp_3.Responses
{
    internal class NotFound : Response
    {

        public NotFound()
        {
            CustomMessage = "No informations";
            StatusCode = 404;
        }

        public NotFound(string message)
        {
            CustomMessage = message;
            StatusCode = 404;
        }
    }
}
