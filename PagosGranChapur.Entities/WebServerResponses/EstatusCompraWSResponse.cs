using Newtonsoft.Json;

namespace PagosGranChapur.Entities.WebServerResponses
{
    public class EstatusCompraWSResponse
    {
        [JsonProperty(PropertyName = "idOperacionCaja")]
        public int IdOperacionCaja { get; set; }

        [JsonProperty(PropertyName = "estatus")]
        public int Estatus { get; set; }

        [JsonProperty(PropertyName = "mensaje")]
        public string Mensaje { get; set; }

    }
}
