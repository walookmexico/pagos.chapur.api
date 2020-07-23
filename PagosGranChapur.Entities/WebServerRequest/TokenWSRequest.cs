using Newtonsoft.Json;

namespace PagosGranChapur.Entities.WebServerRequest
{
    public class TokenWSRequest
    {
        // NUMERO DE TARJETA DE CRÉDTIO
        [JsonProperty(PropertyName = "tarjeta")]
        public string Tarjeta { get; set; }

        // IDENTIFICADOR DE LA TIENDA
        [JsonProperty(PropertyName = "tienda")]
        public int Tienda     { get; set; }

        // IDENTIFIADOR DE LA ACCION A REALIZAR
        [JsonProperty(PropertyName = "accion")]
        public int Accion     { get; set; }

    }
}
