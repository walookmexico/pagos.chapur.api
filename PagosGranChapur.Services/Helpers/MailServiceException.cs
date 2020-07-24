using System;

namespace PagosGranChapur.Entities.Helpers
{
    [Serializable]
    public class MailServiceException : Exception
    {
        public MailServiceException()
        {
        }

        public MailServiceException(string message)
            : base(message)
        {
        }

        public MailServiceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
