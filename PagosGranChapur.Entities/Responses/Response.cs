using System;
using System.Collections.Generic;
using System.Text;

namespace PagosGranChapur.Entities.Responses
{
    public class Response<T> : ResponseBase
    {
        // DATOS DE RESULTADOS
        public T Data { get; set; }       
    }
}
