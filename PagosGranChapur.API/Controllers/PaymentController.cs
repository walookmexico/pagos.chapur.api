using PagosGranChapur.API.Models;
using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Entities.WebServerRequest;
using PagosGranChapur.Entities.WebServerResponses;
using PagosGranChapur.Services;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;

namespace PagosGranChapur.API.Controllers
{
    [RoutePrefix("api/v0/Payments")]
    public class PaymentController : BaseController
    {
       private readonly IPaymentService _srvPayment;

        public PaymentController( IPaymentService srvPayment ) {
            this._srvPayment = srvPayment;
        }

        /// <summary>
        /// API encargada de ejecutar la compra realizada por el usuario
        /// </summary>
        /// <param name="request">Datos obtenidos del cliente</param>
        /// <returns> Response<PaymentWSResponse></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Save")]
        public async Task<IHttpActionResult> Save(SaveOrderPaymentRequest request)
        {
            var response = new Response<PaymentWSResponse>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                response = await this._srvPayment.ApplyPayment(request, ConfigurationManager.AppSettings["Chapur.API.OrdenCompra"],
                                                                ConfigurationManager.AppSettings["Chapur.API.BaseURL"]);

                
                return Ok(response);
                

            }
            catch (System.Exception ex)
            {
                ResponseConverter.SetErrorResponse(response, "Error al realizar el pago, favor de intentar más tarde");
                response.InternalError = ex.Message;

                return Ok(response);
            }

        }

        /// <summary>
        /// API encargada de ejecutar la compra realizada por el usuario enviando el detalle de ella
        /// </summary>
        /// <param name="request">Datos obtenidos del cliente</param>
        /// <returns> Response<PaymentWSResponse></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("detail")]
        public async Task<IHttpActionResult> SaveDetail(SaveDetailPaymentRequest request)
        {
            var response = new Response<PaymentWSResponse>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                response = await this._srvPayment.ApplyPaymentDetail(request, ConfigurationManager.AppSettings["Chapur.API.Detallecompra"],
                                                                     ConfigurationManager.AppSettings["Chapur.API.BaseURL"]);

                return Ok(response);

            }
            catch (System.Exception ex)
            {
                ResponseConverter.SetErrorResponse(response, "Error al realizar el pago, favor de intentar más tarde");
                response.InternalError = ex.Message;

                return Ok(response);
            }
           
        }

        /// <summary>
        /// API encargada de validar el estatus de la autorización 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("checkStatus")]
        public async Task<IHttpActionResult> CheckStatus(EstatusCompraWSRequest request) {

            var response = new Response<EstatusCompraWSResponse>();

            try
            {
                response = await this._srvPayment.CheckStatusPurchaseOrder(request, ConfigurationManager.AppSettings["Chapur.API.EstatusCompra"],
                                                                           ConfigurationManager.AppSettings["Chapur.API.BaseURL"]);

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                ResponseConverter.SetErrorResponse(response, "Error al validar el estatus de la compra");
                response.InternalError = ex.Message;

                return Ok(response);
            }

        }

        /// <summary>
        /// API que obtiene los datos de una tarjeta de crédito filtrada por el id enviado como 
        /// parámetro (solamente es temporal para realizar pruebas.)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("creditcard")]
        public async Task<IHttpActionResult> CreditCard(int id)
        {
            Response<CreditCard> creditCardResponse = await Task.Run(() =>
            {
                var response = new Response<CreditCard>();

                try
                {
                    var creditcardList = new CreditCardsList();
                    response.Data = creditcardList.GetCreditCard(id);
                    response.IsSuccess = true;
                }
                catch (System.Exception ex)
                {
                    ResponseConverter.SetErrorResponse(response, "Error al realizar el pago, favor de intentar más tarde");
                    response.InternalError = ex.Message;
                }

                return response;
            });

            return Ok(creditCardResponse);
        }
    }
}
