using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
