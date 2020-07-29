using PagosGranChapur.Entities.Responses;
using System;

namespace PagosGranChapur.Entities.Helpers
{
    public class ResponseConverter
    {
        private ResponseConverter() { }

        public static Response<T> ToExceptionResponse<T>(Exception ex)
        {
            return new Response<T>
            {
                IsSuccess = false,
                InternalError = ex.Message
            };
        }

        public static void SetErrorResponse<T>(Response<T> response, string message)
        {
            response.IsSuccess = false;
            response.Messages = message;
        }

        public static void SetSuccessResponse<T>(Response<T> response, string message)
        {
            response.IsSuccess = true;
            response.Messages = message;
        }

    }
}
