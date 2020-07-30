using System;
using System.Runtime.Serialization;

namespace PagosGranChapur.Entities.Helpers
{
    [Serializable]
    public sealed class PagosChapurException : Exception
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

        private PagosChapurException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            
        }
    }
}
