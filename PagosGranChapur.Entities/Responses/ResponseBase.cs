namespace PagosGranChapur.Entities.Responses
{
    public class ResponseBase
    {       

        // DETERMINA SI FUE EXITOSA LA OPERACION 
        public bool IsSuccess       { get; set; }

        // MENSAJE DE LA OPERACION REALIZADA
        public string Messages      { get; set; }

        // ERROR INTERNO DE LA OPERACION
        public string InternalError { get; set; }        
        
    }
}
