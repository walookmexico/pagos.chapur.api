using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities.Helpers
{
    public class ValidationPiorpiException: Exception
    {
        public ValidationPiorpiException()
        {
        }

        public ValidationPiorpiException(string message)
            : base(message)
        {
        }

        public ValidationPiorpiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
