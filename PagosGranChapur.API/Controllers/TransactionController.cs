using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Services;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;

namespace PagosGranChapur.API.Controllers
{
    [RoutePrefix("api/v0/Transaction")]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            this._transactionService = transactionService;
        }
        
        /// <summary>
        /// API que permite llenar la tabla de transacciones con datos dummies
        /// </summary>
        /// <returns></returns>
        [Route("loadData")]
        [HttpGet]
        public async Task<IHttpActionResult> LoadData()
        {
            try
            {
                return Ok(
                    await _transactionService.LoadData()
                );
            } 
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtienes los datos de las transacciones filtrada por los parámetros  de fecha de aplicación, identificador de la tienda y
        /// el identificador de la plataforma
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="storeId"></param>
        /// <param name="platformId"></param>
        /// <returns></returns>
        [Route("filter")]
        [HttpGet]
        [Authorize(Roles = "Administrator, Consulting")]
        public async Task<IHttpActionResult> FilterData(DateTime? startDate, DateTime? endDate, string storeId, string platformId)
        {
            try
            {
 
                string result = Validator.ValidateDates(startDate, endDate);
                
                if (result == null)
                {
                    int[] stores = Converter.StringIdToArrayIntId(storeId);
                    int[] plarforms = Converter.StringIdToArrayIntId(platformId);

                    return Ok(
                        await _transactionService.Filter(startDate.Value, endDate, stores, plarforms)
                    );
                } 
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception)
            {                
                return BadRequest();
            }
        }

        /// <summary>
        /// API qe permite obtener el LOG de las transacciones realizadas filtada por la fecha de aplicación
        /// identificador de la tienda y el identificadore de la plataforma
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="storeId"></param>
        /// <param name="platformId"></param>
        /// <returns></returns>
        [Route("status")]
        [HttpGet]
        [Authorize(Roles = "Administrator, Consulting")]
        public async Task<IHttpActionResult> FilterStatus(DateTime? startDate, DateTime? endDate, string storeId, string platformId)
        {
            try
            {

                string result = Validator.ValidateDates(startDate, endDate);

                if (result == null)
                {
                    int[] stores = Converter.StringIdToArrayIntId(storeId);
                    int[] plarforms = Converter.StringIdToArrayIntId(platformId);

                    return Ok(
                        await _transactionService.FilterLog(startDate.Value, endDate, stores, plarforms)
                    );
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// API para obtener el identificador de operación de caja generado por la compra de todas las transacciones
        /// en un rango de fecha determinado
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [Route("checkStatusByDate")]
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IHttpActionResult> CheckStatusByDate(DateTime? startDate, DateTime? endDate)
        {
            try
            {                

                return Ok(
                    await _transactionService.CheckStatusTransactions(startDate.Value, endDate, ConfigurationManager.AppSettings["Chapur.API.EstatusCompra"],
                                                                             ConfigurationManager.AppSettings["Chapur.API.BaseURL"])
                    );
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// API para obtener el identificador de operación de caja generado por la compra de todas las transacciones
        /// que no tengan este identificador
        /// </summary>
        /// <returns></returns>
        [Route("checkStatus")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> CheckStatusWithOutId()
        {
            try
            {

                return Ok(
                    await _transactionService.CheckStatusTransactionsWithOutId(ConfigurationManager.AppSettings["Chapur.API.EstatusCompra"],
                                                                                      ConfigurationManager.AppSettings["Chapur.API.BaseURL"])
                    );
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}