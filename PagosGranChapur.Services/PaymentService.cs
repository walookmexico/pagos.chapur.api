using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Enums;
using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Entities.WebServerRequest;
using PagosGranChapur.Entities.WebServerResponses;
using PagosGranChapur.Repositories;
using PagosGranChapur.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PagosGranChapur.Services
{
    public interface IPaymentService
    {
        Task<Response<PaymentWSResponse>> ApplyPayment(SaveOrderPaymentRequest request, string apiUrl, string baseURL);
        Task<Response<PaymentWSResponse>> ApplyPaymentDetail(SaveDetailPaymentRequest request, string apiUrl, string baseURL);
        Task<Response<EstatusCompraWSResponse>> CheckStatusPurchaseOrder(EstatusCompraWSRequest request, string apiUrl, string baseURL);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogTransactionRepository _logTransactionRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IPlatformRepository _platformRepository;

        public PaymentService(IUnitOfWork unitOfWork,
            ITransactionRepository transactionRepository,
            ILogTransactionRepository logTransactionRepository,
            IStoreRepository storeRepository,
            IPlatformRepository platformRepository)
        {
            this._unitOfWork = unitOfWork;
            this._transactionRepository = transactionRepository;
            this._logTransactionRepository = logTransactionRepository;
            this._storeRepository = storeRepository;
            this._platformRepository = platformRepository;
        }

        #region INTERFACE METHODS

        /// <summary>
        /// Método que realiza la compra de una orden de compra, por medio del servicio web expuesto por CHAPUR y el resultado es 
        /// almacenado en la tabla de transacciones del API
        /// </summary>
        /// <param name="request"> Datos requeridos para ejecutar la compra </param>
        /// <returns>Response<PaymentWSResponse></returns>
        public async Task<Response<PaymentWSResponse>> ApplyPayment(SaveOrderPaymentRequest request, string apiUrl, string baseURL)
        {
            var response    = new Response<PaymentWSResponse>();
            bool success = true;

            try
            {
                //VALIDAR SI LA ORDEN DE COMPRA YA TIENE UNA TRANSACCION ASIGNADA
                response = await GetResponseValidateExistTransaction(request.IdPurchaseOrder, request.Amount);

                if (response != null)
                {
                    if(response.Messages != null)
                    {
                        success = false;
                    }
                }

                if(success == true)
                {
                    response = await SavePurchaseOrder(response, request, apiUrl, baseURL);
                }
            }           
            catch (System.Net.Http.HttpRequestException exHttpRequest)  //EXCEPCIÓN DE CONEXIÓN CON EL SERVIDOR DE SERVICIOS DE CHAPUR
            {
                await SaveLog(request, -101, "Error al realizar conexión con los servicios de GRAN CAHPUR:" + exHttpRequest.Message);

                SetErrorPurchaseOrder(response, exHttpRequest.Message);
            }
            catch (Exception ex) //EXCEPCIÓN DE TODOS LOS TIPOS POSIBLES DE ERRORES
            {

                SetErrorPurchaseOrder(response, ex);

                await SaveLog(request, -102, "Error al ejecutar el pago, favor de intentar más tarde:" + response.InternalError);

            }

            return response;
        }

        private async Task<Response<PaymentWSResponse>> GetResponseValidateExistTransaction(string idPurchaseOrder, decimal amount)
        {
            var response = new Response<PaymentWSResponse>();

            var isExistTransactions = await ValidateExistTransaction(idPurchaseOrder, amount);

            if (isExistTransactions != null)
            {
                if (isExistTransactions.Amount == amount)
                {
                    response.Data = new PaymentWSResponse
                    {
                        Estatus = 0,
                        IdAutorizacion = isExistTransactions.AutorizationId,
                        IdPlatadorma = isExistTransactions.PlatformId,
                        FechaHoraAplicacion = isExistTransactions.CreateDate,
                        NombreCuentaHabiente = isExistTransactions.NameCreditCard,
                        DescripcionTienda = isExistTransactions.StoreDescripcion
                    };

                    ResponseConverter.SetSuccessResponse(response, "Ya existe una transacion registrada");
                }
                else
                {
                    response.Data = null;
                    ResponseConverter.SetErrorResponse(response, "Existe una transaccion registrada con el mismo orden de compra pero diferente total, favor de revisar este dato");
                }
            }

            return response;
        }

        private async Task<Response<PaymentWSResponse>> SavePurchaseOrder(Response<PaymentWSResponse> response, SaveOrderPaymentRequest request, string apiUrl, string baseURL)
        {
            //ASIGNAR LO VALORES AL OBJETO REQUERIDO POR EL SERVICIO
            var dataService = PaymentConverter.OrderPaymentToOrdenCompra(request);

            var errorPiorpi = await ValidatePIORPI(request.NameCreditCard, request.Amount);

            dataService.ReglaPrp = (int)EnumPiorpi.Correcto;

            if (errorPiorpi.Count > 0)
                dataService.ReglaPrp = (int)EnumPiorpi.Pendiente;

            if (await ActivateAlertPiorpi(request.NameCreditCard))
                dataService.ReglaPrp = (int)EnumPiorpi.Alerta;


            dataService.DetalleValidacionesPrp = errorPiorpi;

            var purchaseOrderResponse = await RequestService.Post<OrdenCompraRequest, PaymentWSResponse>(apiUrl, baseURL, dataService);

            //VALIDAR LA RESPUESTA DEL SERVICIO
            if (purchaseOrderResponse != null)
            {
                await ValidateResponse(response, purchaseOrderResponse, request, dataService.ReglaPrp, errorPiorpi);
            }
            else
            {
                ResponseConverter.SetErrorResponse(response, "Error de conexión con tiendas CHAPUR.");
            }

            return response;
        }

        private async Task ValidateResponse(Response<PaymentWSResponse> response, PaymentWSResponse purchaseOrderResponse, SavePaymentRequest request,  int reglaPrp, List<ValidacionPrp> errorPiorpi)
        {
           
            if (purchaseOrderResponse.Estatus > 0)
            {
                response = await SaveTransaction(purchaseOrderResponse, request, reglaPrp, errorPiorpi);
                ResponseConverter.SetSuccessResponse(response, "Pago aplicado correctamente");

                await AddCatalog(purchaseOrderResponse);
            }
            else
            {
                response.Data = new PaymentWSResponse
                {
                    Estatus = purchaseOrderResponse.Estatus.Value,
                    Mensaje = purchaseOrderResponse.Mensaje,
                };
                ResponseConverter.SetErrorResponse(response, "Problemas al ejecutar el servicio de pago");

                await SaveLog(request, purchaseOrderResponse.Estatus.Value, purchaseOrderResponse.Mensaje);
            }
            
        }

        private void SetErrorPurchaseOrder(Response<PaymentWSResponse> response, String message)
        {
            ResponseConverter.SetErrorResponse(response, "Error al realizar conexión con tiendas CHAPUR, favor de intentar más tarde");
            response.InternalError = message;
        }

        private void SetErrorPurchaseOrder(Response<PaymentWSResponse> response, Exception ex)
        {
            if (ex.InnerException != null)
            {
                if (ex.InnerException.InnerException != null)
                    response.InternalError = ex.InnerException.InnerException.Message;
                else
                    response.InternalError = ex.InnerException.Message;
            }
            else
                response.InternalError = ex.Message;

            ResponseConverter.SetErrorResponse(response, "Error al ejecutar el pago, favor de intentar más tarde");
        }

        /// <summary>
        /// Método que realiza la compra detalle por medio del servicio web expuesto por CHAPUR y el resultado es
        /// almacenado en la tabla de transacciones del API
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiUrl"></param>
        /// <param name="baseURL"></param>
        /// <returns></returns>
        public async Task<Response<PaymentWSResponse>> ApplyPaymentDetail(SaveDetailPaymentRequest request, string apiUrl, string baseURL)
        {
            var response = new Response<PaymentWSResponse>();

            try
            {
                if (request.Detail == null)
                {
                    ResponseConverter.SetSuccessResponse(response, "El detalle de la compra es requerido");

                    return response;
                }

                var dataService = PaymentConverter.SaveDetailPaymentToOrdenCompraDetalle(request);

                var errorPiorpi = await ValidatePIORPI(request.NameCreditCard, request.Amount);

                SetReglaPrp(dataService, errorPiorpi);

                dataService.DetalleValidacionesPrp = errorPiorpi;

                var purchaseOrderResponse = await RequestService.Post<OrdenCompraDetalleRequest, PaymentWSResponse>(apiUrl, baseURL, dataService);

                //VALIDAR LA RESPUESTA DEL SERVICIO
                if (purchaseOrderResponse != null)
                {
                    await ValidateResponse(response, purchaseOrderResponse, request, dataService.ReglaPrp, errorPiorpi);
                }
                else
                {
                    ResponseConverter.SetErrorResponse(response, "Error de conexión con el Servicio de Pago");
                }

            }           
            catch (System.Net.Http.HttpRequestException exHttpRequest)
            {
                await SaveLog(request, -101, "Error al realizar conexión con los servicios de GRAN CHAPUR:" + exHttpRequest.Message);

                SetErrorPurchaseOrder(response, exHttpRequest);
            }
            catch (Exception ex)
            {
                SetErrorPurchaseOrder(response, ex);

                await SaveLog(request, -102, "Error al ejecutar el pago, favor de intentar más tarde:" + response.InternalError);
            }

            return response;
        }

        private void SetReglaPrp(OrdenCompraDetalleRequest dataService, List<ValidacionPrp> errorPiorpi)
        {
            switch (errorPiorpi.Count)
            {
                case 0:
                    dataService.ReglaPrp = (int)EnumPiorpi.Correcto;
                    break;
                case 1:
                case 2:
                    dataService.ReglaPrp = (int)EnumPiorpi.Pendiente;
                    break;
                default:
                    dataService.ReglaPrp = (int)EnumPiorpi.Alerta;
                    break;
            }
        }

        /// <summary>
        /// Método que permite validar el estatus de una compra
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiUrl"></param>
        /// <param name="baseURL"></param>
        /// <returns></returns>
        public async Task<Response<EstatusCompraWSResponse>> CheckStatusPurchaseOrder(EstatusCompraWSRequest request, string apiUrl, string baseURL)
        {

            var response = new Response<EstatusCompraWSResponse>();

            try
            {
                var statusPurchaseOrder = await RequestService.Post<EstatusCompraWSRequest, EstatusCompraWSResponse>(apiUrl, baseURL, request);
                response.Data = statusPurchaseOrder;
                ResponseConverter.SetSuccessResponse(response, "Autorización validada correctamente");

            }
            catch (Exception ex)
            {
                response.Data = null;
                ResponseConverter.SetErrorResponse(response, "Error al intentar de validar el estatus de la compra");
                response.InternalError = ex.Message;
            }
            return response;

        }


        #endregion

        #region METHODS PRIVATE

        /// <summary>
        /// Registrar la respuesta del servicio en la base de datos
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<Response<PaymentWSResponse>> SaveTransaction(PaymentWSResponse response, 
                                                                        SavePaymentRequest request, int statusPiorpi, 
                                                                        List<ValidacionPrp> errores)
        {

            var result = new Response<PaymentWSResponse>();

            var transacction = PaymentConverter.ToTransaction(response, request);

            transacction.StatusPiorpi = statusPiorpi;

            if (statusPiorpi == 2) {

                var detail = "";
                errores.ForEach(e => detail += $"{e.Descripcion}");
                transacction.DetailPiorpi = detail;

            }
            else if (statusPiorpi == 3) {
                transacction.DetailPiorpi = "Tiene más de 3 transaciones en estatus pendiente";
            }

            var logTransaction = PaymentConverter.ToLogTransaction(transacction, response);

            await _transactionRepository.AddAsync(transacction);
            await _logTransactionRepository.AddAsync(logTransaction);
            await _unitOfWork.SaveChangesAsync();

            result.Data = PaymentConverter.LogTransactionToPaymentResponse(logTransaction, transacction);

            return result;
        }

        /// <summary>
        /// Registra la transaccion cuando es declinada por las reglas de PIORPO
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task SaveTransaction(SavePaymentRequest request, string message)
        {

            var transaction = new Transaction
            {
                NameCreditCard  = request.NameCreditCard,
                UserId          = 0,
                PlatformId      = request.PlatformId > 0 ? request.PlatformId : 0,
                Amount          = decimal.Parse(request.Amount.ToString()),
                CreateDate      = DateTime.Now,
                PurchaseOrderId = request.IdPurchaseOrder,
                StoreId         = request.IdStore > 0 ? request.IdStore : 0,
                Email           = request.Email
            };

            var logTransaction = new LogTransaction
            {
                DateApply       = DateTime.Now,
                MessageResponse = message,
                PurchaseOrderID = transaction.PurchaseOrderId,
                Status          = -103,
                TransactionId   = transaction.Id,
                PlatformId      = transaction.PlatformId
            };

            await _transactionRepository.AddAsync(transaction);
            await _logTransactionRepository.AddAsync(logTransaction);
            await _unitOfWork.SaveChangesAsync();
            
        }

        /// <summary>
        /// Validar si la transacción ya fue ejecutada y guardada en la base de datos
        /// </summary>
        /// <param name="purchaseOrderId"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private async Task<Transaction> ValidateExistTransaction(string purchaseOrderId, decimal total)
        {
            try
            {
                var transacction = await _transactionRepository.FindAsync(x => x.PurchaseOrderId == purchaseOrderId);

                if (transacction != null)
                    return transacction;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// GUARDAR DATOS DE ERROR EN EL LOG DE TRANSACCIONES
        /// </summary>
        /// <param name="request"></param>
        /// <param name="codeStatus"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private async Task<bool> SaveLog(SavePaymentRequest request, int codeStatus, string error)
        {
            try
            {
                var log = new LogTransaction
                {
                    MessageResponse = error,
                    PurchaseOrderID = request.IdPurchaseOrder,
                    Status          = codeStatus,
                    DateApply       = DateTime.Now,
                    PlatformId      = request.PlatformId
                };

                await _logTransactionRepository.AddAsync(log);
                await _unitOfWork.SaveChangesAsync();

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// MÉTODO QUE PERMITE VALIDAR Y AGREGAR UNA NUEVA PLATAFORMA O TIENDA AL CATÁLOGO
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task AddCatalog(PaymentWSResponse response)
        {
            try
            {
                var store    = _storeRepository.Get(x => x.Id == response.IdTienda);
                var platform = _platformRepository.Get(x => x.Id == response.IdPlatadorma);

                if (store == null && response.IdTienda != null)
                {
                    _storeRepository.Add(new Store
                    {
                        Description = response.DescripcionTienda,
                        Id          = response.IdTienda.Value
                    });
                }

                if (platform == null && response.IdPlatadorma != null)
                {
                    _platformRepository.Add(new Platform
                    {
                        Description = response.DescripcionPlataforma,
                        Id          = response.IdPlatadorma.Value
                    });
                }

                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception)
            {
                response.Mensaje = "Problemas al agregar el catálogo";
            }
        }

        /// <summary>
        /// VALIDACIÓN DE REGLAS PIORPI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<List<ValidacionPrp>> ValidatePIORPI(string name, decimal total)
        {
            List<ValidacionPrp> validacions = await Task.Run(() =>
               {
                   var response = new List<ValidacionPrp>();
                   var starMonthtDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                   var lastMonthDay = (new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(1).Month, 1).AddDays(-1));
                   var monday = FirstDateInWeek(DateTime.Today);
                   var sunday = LastDayOfWeek(DateTime.Today);

                   var transactionsMonth = _transactionRepository.GetMany(t => t.NameCreditCard == name && (t.CreateDate >= starMonthtDay && t.CreateDate <= lastMonthDay) && t.AutorizationId != null).ToList();
                   var transactionsDay = transactionsMonth.Where(t => t.CreateDate.Date == DateTime.Today.Date).ToList();
                   var trasactionsWeek = transactionsMonth.Where(t => t.CreateDate.Date >= monday.Date && t.CreateDate.Date <= sunday.Date).ToList();

                   var limitDay = decimal.Parse(ConfigurationManager.AppSettings["PIORPI.Limit.Day"].ToString());
                   var limitWeek = decimal.Parse(ConfigurationManager.AppSettings["PIORPI.Limit.Week"].ToString());
                   var limitYear = decimal.Parse(ConfigurationManager.AppSettings["PIORPI.Limit.Month"].ToString());
                   var totalDia = transactionsDay.Sum(t => t.Amount) + total;
                   var totalWeek = trasactionsWeek.Sum(t => t.Amount) + total;
                   var totakMonth = transactionsMonth.Sum(t => t.Amount) + total;

                // Al solicitar una o más transacciones que sumen 40 mil pesos o mayor en un día, se requiere una validación con el cliente.
                if (totalDia >= limitDay)
                   {
                       response.Add(new ValidacionPrp($"Limite díario de {limitDay.ToString("C")} superado, se requiere aprobación por parte del cliente", EnumReglaPiorpi.LimiteExcedidoMontoDiario));
                   }

                // Al solicitar más de 2 transacciones en un solo día, se requiere una validación con el cliente.
                if (transactionsDay.Count >= 2)
                   {
                       response.Add(new ValidacionPrp("Se supero el número de compras diarios, se requiere aprobación por parte del cliente", EnumReglaPiorpi.LimiteComprasPorDia));
                   }

                // Al solicitar transacciones que sumen 80 mil pesos o mas en una semana, se requiere una validación con el cliente.
                if (totalWeek >= limitWeek)
                   {
                       response.Add(new ValidacionPrp($"Se supero el limite de compras semanal de {limitWeek.ToString("C")}, se requiere aprobación por parte del cliente", EnumReglaPiorpi.LimiteExcedidoMontoSemanal));
                   }

                // Al solicitar una o más transacciones que sumen 100 mil pesos o mas en un mes, se requiere una validación con el cliente.
                if (totakMonth >= limitYear)
                   {
                       response.Add(new ValidacionPrp($"Se supero el limite de compras mensual de {limitYear.ToString("C")}, se requiere aprobación por parte del cliente", EnumReglaPiorpi.LimiteExcedidoMontoMensual));
                   }

                   return response;
               });
            return validacions;
        }

        /// <summary>
        /// Valida los el estatus de las reglas de piorpi para determinar si activar el estatus alerta para 
        /// la transaccion
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<bool> ActivateAlertPiorpi(string name) {

            var transactions = await _transactionRepository.FindAllAsync(x => x.NameCreditCard == name && x.StatusPiorpi == (int)EnumPiorpi.Pendiente);

            if (transactions.Count >= 3)
                return true;
            else
                return false;
        }

        private DateTime FirstDateInWeek(DateTime dt)
        {
            DayOfWeek fdow    = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            int offset        = fdow - dt.DayOfWeek;
            DateTime fdowDate = dt.AddDays(offset);

            return fdowDate;
        }

        private DateTime LastDayOfWeek(DateTime date)
        {
            DateTime ldowDate = FirstDateInWeek(date).AddDays(6);
            return ldowDate;
        }

        #endregion
    }
}
