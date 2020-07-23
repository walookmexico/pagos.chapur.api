using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PagosGranChapur.Entities.WebServerRequest
{
    public class EstatusCompraWSRequest
    {
        [Required]
        [JsonProperty(PropertyName = "idAutorizacion")]
        public int IdAutorizacion { get; set; }
        [Required]
        [JsonProperty(PropertyName = "idTienda")]
        public int IdTienda       { get; set; }
    }
}
