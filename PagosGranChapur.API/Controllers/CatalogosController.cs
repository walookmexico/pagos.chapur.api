using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Entities.WebServerResponses;
using PagosGranChapur.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PagosGranChapur.API.Controllers
{
    [RoutePrefix("api/v0/Catalogs")]
    public class CatalogosController : BaseController
    {
        private readonly IStoreService _storeService;
        public readonly  IPlatformService _platformService;

        public CatalogosController(IStoreService storeService,
             IPlatformService platformService)
        {
            this._storeService    = storeService;
            this._platformService = platformService;

        }

        /// <summary>
        /// API QUE OBTIENE LOS CATÁLOGOS DE TIENDAS Y PLATAFORMAS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Dashboard")]
        public async Task<IHttpActionResult> Dashboard()
        {
            var response = new Response<DashBoardResponse>();

            try
            {
                var stores    = await _storeService.GetAll();
                var platforms = await _platformService.GetAll();

                response.Data = new DashBoardResponse
                {
                    Platforms = platforms,
                    Stores = stores
                };

                response.IsSuccess = true;
                response.Messages = "Datos listados correctamente";

                return Ok(response);

            }
            catch (System.Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Error al obtener los datos";
                response.InternalError = ex.Message;

                return Ok(response);
            }

        }

    }
}
