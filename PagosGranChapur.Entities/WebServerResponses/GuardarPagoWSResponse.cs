using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
