using System;

namespace PagosGranChapur.Entities.Helpers
{
    [Serializable]
    public class PagosChapurException : Exception
    {
        public PagosChapurException()
        {
        }

        public PagosChapurException(string message)
            : base(message)
        {
        }

        public PagosChapurException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
