using System;

namespace PagosGranChapur.Entities.Helpers
{
    [Serializable]
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
