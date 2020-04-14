using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
