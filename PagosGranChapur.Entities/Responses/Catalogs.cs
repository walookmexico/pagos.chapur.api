using System.Collections.Generic;

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
