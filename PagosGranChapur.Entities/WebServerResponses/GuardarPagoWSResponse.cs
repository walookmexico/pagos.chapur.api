using Newtonsoft.Json;

namespace PagosGranChapur.Entities.WebServerResponses
{
    public class GuardarPagoWSResponse
    {
        [JsonProperty(PropertyName = "estadoCompra")]
        public bool EstadoCompra  { get; set; }
        
        [JsonProperty(PropertyName = "idRegistroPago")]
        public int IdRegistroPago { get; set; }
    }
}
