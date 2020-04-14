using PagosGranChapur.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities.Helpers
{
    public class ErrorHandler
    {
        public ErrorHandler(string message, EnumError type) {
            this.Message = message;
            this.Type    = type;
        }

        public string Message   { get; set; }
        public EnumError Type   { get; set; }

    }
}
