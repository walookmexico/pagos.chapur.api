using Newtonsoft.Json;

namespace PagosGranChapur.Entities.WebServerResponses
{
    public class TokenWSResponse
    {
        // ESTATUS DE LA RESPUESTA DEL SERVICIO
        [JsonProperty(PropertyName = "estatus")]
        public int Estatus     { get; set; }

        // TOKEN GENERADO POR CHAPUR PARA CONFIRMAR LA COMPRA
        [JsonProperty(PropertyName = "token")]
        public string Token    { get; set; }

        // NÚMERO DE CELUAR RELACIONADO A LA TARJETA DE CRÉDITO
        [JsonProperty(PropertyName = "telefono")]
        public string Telefono { get; set; }

        // MENSAJE DE RESPUESTA 
        [JsonProperty(PropertyName = "mensaje")]
        public string Mensaje  { get; set; }

        // CORREO ELECTRÓNICO DEL CLIENTE
        [JsonProperty(PropertyName = "correo")]
        public string CorreoElectronico { get; set; }
    }
}
