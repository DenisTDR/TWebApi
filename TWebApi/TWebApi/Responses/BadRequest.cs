using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace customApiApp_3.Responses
{
    public class BadRequest : Response
    {
        public BadRequest()
        {
            CustomMessage = "No informations";
            StatusCode = 400;
        }

        public BadRequest(string message)
        {
            CustomMessage = message;
            StatusCode = 400;
        }
    }
}
