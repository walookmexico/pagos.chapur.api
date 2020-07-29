using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Services;
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

                ResponseConverter.SetSuccessResponse(response, "Datos listados correctamente");

                return Ok(response);

            }
            catch (System.Exception ex)
            {
                ResponseConverter.SetErrorResponse(response, "Error al obtener los datos");
                response.InternalError = ex.Message;

                return Ok(response);
            }

        }

    }
}
