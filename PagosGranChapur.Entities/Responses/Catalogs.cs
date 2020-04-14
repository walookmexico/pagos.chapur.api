using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities.Responses
{
    public class DashBoardResponse
    {
        // LISTADO DE TIENDAS
        public List<Store> Stores       { get; set; }

        // LISTADO DE PLATAFORMAS
        public List<Platform> Platforms { get; set; }
    }
}
