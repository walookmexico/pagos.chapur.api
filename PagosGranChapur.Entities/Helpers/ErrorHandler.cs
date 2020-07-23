using PagosGranChapur.Entities.Enums;

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
