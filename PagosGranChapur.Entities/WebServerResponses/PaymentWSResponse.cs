using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PagosGranChapur.Entities.WebServerResponses
{
    public class PaymentWSResponse
    {

        [JsonProperty(PropertyName = "estatus")]
        public int? Estatus                  { get; set; }

        [JsonProperty(PropertyName = "mensaje")]
        public string Mensaje               { get; set; }

        [JsonProperty(PropertyName = "fechaHoraAplicacion")]
        public DateTime? FechaHoraAplicacion { get; set; }

        [JsonProperty(PropertyName = "monto")]
        public float? Monto                { get; set; }

        [JsonProperty(PropertyName = "nombreCuentaHabiente")]
        public string NombreCuentaHabiente  { get; set; }

        [JsonProperty(PropertyName = "idAutorizacion")]
        public string IdAutorizacion           { get; set; }

        [JsonProperty(PropertyName = "idTienda")]
        public int? IdTienda                 { get; set; }

        [JsonProperty(PropertyName = "descripcionTienda")]
        public string DescripcionTienda     { get; set; }

        [JsonProperty(PropertyName = "idplataforma")]
        public int? IdPlatadorma             { get; set; }

        [JsonProperty(PropertyName = "descripcionPlataforma")]
        public string DescripcionPlataforma { get; set; }

        [JsonProperty(PropertyName = "idUsuario")]
        public int? IdUsuario                { get; set; }

    }
}
