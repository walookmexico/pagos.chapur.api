﻿using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Entities.WebServerRequest;
using PagosGranChapur.Entities.WebServerResponses;
using PagosGranChapur.Services.Helpers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using PagosGranChapur.Repositories;
using System.Globalization;
using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Entities.Enums;

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
            var errorPiorpi = new List<ValidacionPrp>();

            try
            {
                //VALIDAR SI LA ORDEN DE COMPRA YA TIENE UNA TRANSACCION ASIGNADA
                var isExistTransactions = await this.ValidateExistTransaction(request.IdPurchaseOrder, request.Amount);

                if (isExistTransactions != null)
                {
                    if (isExistTransactions.Amount == request.Amount)
                    {
                        response.Data = new PaymentWSResponse
                        {
                            Estatus              = 0,
                            IdAutorizacion       = isExistTransactions.AutorizationId,
                            IdPlatadorma         = isExistTransactions.PlatformId,
                            FechaHoraAplicacion  = isExistTransactions.CreateDate,
                            NombreCuentaHabiente = isExistTransactions.NameCreditCard,
                            DescripcionTienda    = isExistTransactions.StoreDescripcion
                        };

                        response.IsSuccess = true;
                        response.Messages  = "Ya existe una transacion registrada";

                        return response;
                    }
                    else
                    {
                        response.Data      = null;
                        response.IsSuccess = false;
                        response.Messages  = "Existe una transaccion registrada con el mismo orden de compra pero diferente total, favor de revisar este dato";

                        return response;
                    }
                }
                                
                //ASIGNAR LO VALORES AL OBJETO REQUERIDO POR EL SERVICIO
                var dataService = new OrdenCompraRequest
                {
                    CorreoElectronico    = request.Email,
                    CVV                  = request.CVV,
                    FechaAlta            = request.CreateDate,
                    IdOrden              = request.IdPurchaseOrder,
                    IdPlataforma         = request.PlatformId,
                    IdTienda             = request.IdStore,
                    Monto                = (float)request.Amount,
                    NombreCuentaHabiente = request.NameCreditCard,
                    NoTarjeta            = request.NoCreditCard,
                    PasswordPlataforma   = request.PasswordPlatform,
                    Token                = request.Token,
                    UsuarioPlataforma    = request.UserPlatform,
                    Meses                = request.Months.Value,
                    ConInteres           = request.WithInterest.Value
                };
                
                errorPiorpi = await this.ValidatePIORPI(request.NameCreditCard, request.Amount);                                

                dataService.ReglaPrp = (int)EnumPiorpi.Correcto;

                if (errorPiorpi.Count > 0)
                    dataService.ReglaPrp = (int)EnumPiorpi.Pendiente;

                if(await ActivateAlertPiorpi(request.NameCreditCard))
                    dataService.ReglaPrp = (int)EnumPiorpi.Alerta;                
                
                
                dataService.DetalleValidacionesPrp = errorPiorpi;
                
                var purchaseOrderResponse = await RequestService.Post<OrdenCompraRequest, PaymentWSResponse>(apiUrl, baseURL, dataService);

                //VALIDAR LA RESPUESTA DEL SERVICIO
                if (purchaseOrderResponse != null && purchaseOrderResponse.Estatus > 0)
                {
                    response           = await this.SaveTransaction(purchaseOrderResponse, request, dataService.ReglaPrp, errorPiorpi);
                    response.IsSuccess = true;
                    response.Messages  = "Pago aplicado correctamente";

                    
                    await this.AddCatalog(purchaseOrderResponse);
                }
                else
                {
                    response.Data = new PaymentWSResponse
                    {
                        Estatus = purchaseOrderResponse.Estatus.Value,
                        Mensaje = purchaseOrderResponse.Mensaje,
                    };
                    response.IsSuccess = false;
                    response.Messages = "Problemas al ejecutar el servicio de pago";

                    await this.SaveLog(request, purchaseOrderResponse.Estatus.Value, purchaseOrderResponse.Mensaje);
                }                
            }           
            catch (System.Net.Http.HttpRequestException exHttpRequest)  //EXCEPCIÓN DE CONEXIÓN CON EL SERVIDOR DE SERVICIOS DE CHAPUR
            {
                await this.SaveLog(request, -101, "Error al realizar conexión con los servicios de GRAN CAHPUR:" + exHttpRequest.Message);

                response.IsSuccess     = false;
                response.Messages      = "Error al realizar conexión con tiendas CAHPUR, favor de intentar más tarde";
                response.InternalError = exHttpRequest.Message;
            }
            catch (Exception ex) //EXCEPCIÓN DE TODOS LOS TIPOS POSIBLES DE ERRORES
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

                response.IsSuccess = false;
                response.Messages = "Error al ejecutar el pago, favor de intentar más tarde";

                await this.SaveLog(request, -102, "Error al ejecutar el pago, favor de intentar más tarde:" + response.InternalError);

            }

            return response;
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
            var response    = new Response<PaymentWSResponse>();
            var errorPiorpi = new List<ValidacionPrp>();

            try
            {
                if (request.Detail == null)
                {
                    response.IsSuccess = false;
                    response.Messages = "El detalle de la compra es requerido";

                    return response;
                }

                var detail = new List<DetalleCompra>();

                request.Detail.ForEach(d =>
                {
                    detail.Add(new DetalleCompra
                    {
                        Cantidad = d.Quantity,
                        CostoPromedio = d.AverageCost,
                        Descuento = d.Discount,
                        IdArticulo = d.ItemId,
                        IEPS = d.IEPS,
                        IVA = d.IVA,
                        PrecioSinImpuesto = d.PriceWithoutTax
                    });
                });

                
                var dataService = new OrdenCompraDetalleRequest
                {
                    CorreoElectronico    = request.Email,
                    CVV                  = request.CVV,
                    FechaAlta            = request.CreateDate,
                    IdOrden              = request.IdPurchaseOrder,
                    IdPlataforma         = request.PlatformId,
                    IdTienda             = request.IdStore,
                    Monto                = (float)request.Amount,
                    NombreCuentaHabiente = request.NameCreditCard,
                    NoTarjeta            = request.NoCreditCard,
                    PasswordPlataforma   = request.PasswordPlatform,
                    Token                = request.Token,
                    UsuarioPlataforma    = request.UserPlatform,
                    Meses                = request.Months.Value,
                    ConInteres           = request.WithInterest.Value,
                    ListadoDetalle       = detail
                };

                errorPiorpi = await this.ValidatePIORPI(request.NameCreditCard, request.Amount);
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


                dataService.DetalleValidacionesPrp = errorPiorpi;

                var purchaseOrderResponse = await RequestService.Post<OrdenCompraDetalleRequest, PaymentWSResponse>(apiUrl, baseURL, dataService);

                //VALIDAR LA RESPUESTA DEL SERVICIO
                if (purchaseOrderResponse != null && purchaseOrderResponse.Estatus > 0)
                {
                    response = await this.SaveTransaction(purchaseOrderResponse, request, dataService.ReglaPrp, errorPiorpi);
                    response.IsSuccess = true;
                    response.Messages = "Pago aplicado correctamente";

                    await this.AddCatalog(purchaseOrderResponse);
                }
                else
                {
                    response.Data = new PaymentWSResponse
                    {
                        Estatus = purchaseOrderResponse.Estatus.Value,
                        Mensaje = purchaseOrderResponse.Mensaje,
                    };
                    response.IsSuccess = false;
                    response.Messages = "Problemas al ejecutar el servicio de pago";

                    await this.SaveLog(request, purchaseOrderResponse.Estatus.Value, purchaseOrderResponse.Mensaje);
                }

                
            }           
            catch (System.Net.Http.HttpRequestException exHttpRequest)
            {
                await this.SaveLog(request, -101, "Error al realizar conexión con los servicios de GRAN CAHPUR:" + exHttpRequest.Message);

                response.IsSuccess = false;
                response.Messages = "Error al realizar conexión con tiendas CAHPUR, favor de intentar más tarde";
                response.InternalError = exHttpRequest.Message;
            }
            catch (Exception ex)
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

                response.IsSuccess = false;
                response.Messages = "Error al ejecutar el pago, favor de intentar más tarde";

                await this.SaveLog(request, -102, "Error al ejecutar el pago, favor de intentar más tarde:" + response.InternalError);

            }

            return response;
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
                response.IsSuccess = true;
                response.Messages = "Autorización validada correctamente";

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Data = null;
                response.Messages = "Error al intentar de validar el estatus de la compra";
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

            var transacction = new Entities.Transaction
            {
                DecriptionPlatform  = response.DescripcionPlataforma,
                NameCreditCard      = request.NameCreditCard,
                StoreDescripcion    = response.DescripcionTienda,
                UserId              = response.IdUsuario != null ? response.IdUsuario.Value : 0,
                PlatformId          = response.IdPlatadorma != null ? response.IdPlatadorma.Value : 0,
                AutorizationId      = response.IdAutorizacion != null ? response.IdAutorizacion.ToString() : "0",
                Amount              = response.Monto != null ? decimal.Parse(response.Monto.Value.ToString()) : 0,
                CreateDate          = response.FechaHoraAplicacion != null ? response.FechaHoraAplicacion.Value : DateTime.Now,
                PurchaseOrderId     = request.IdPurchaseOrder,
                StoreId             = response.IdTienda != null ? response.IdTienda.Value : 0,
                Email               = request.Email,
                StatusPiorpi        = statusPiorpi,
                Months              = request.Months.Value,
                WithInterest        = request.WithInterest.Value,

            };

            if (statusPiorpi == 2) {

                var detail = "";
                errores.ForEach(e => detail += $"{e.Descripcion}");
                transacction.DetailPiorpi = detail;

            }
            else if (statusPiorpi == 3) {

                transacction.DetailPiorpi = "Tiene más de 3 transaciones en estatus pendiente";
            }            

            var logTransaction = new LogTransaction
            {
                DateApply       = transacction.CreateDate,
                MessageResponse = response.Mensaje,
                PurchaseOrderID = transacction.PurchaseOrderId,
                Status          = response.Estatus != null ? response.Estatus.Value : 0,
                TransactionId   = transacction.Id,
                PlatformId      = transacction.PlatformId
            };

            await this._transactionRepository.AddAsync(transacction);
            await this._logTransactionRepository.AddAsync(logTransaction);
            await this._unitOfWork.SaveChangesAsync();

            result.Data = new PaymentWSResponse
            {
                Estatus               = logTransaction.Status,
                IdAutorizacion        = transacction.AutorizationId,
                IdPlatadorma          = transacction.PlatformId,
                Mensaje               = logTransaction.MessageResponse,
                FechaHoraAplicacion   = transacction.CreateDate,
                Monto                 = (float)transacction.Amount,
                NombreCuentaHabiente  = transacction.NameCreditCard,
                DescripcionTienda     = transacction.StoreDescripcion,
                DescripcionPlataforma = transacction.DecriptionPlatform
            };

            return result;
        }

        /// <summary>
        /// Registra la transaccion cuando es declinada por las reglas de PIORPO
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task SaveTransaction(SavePaymentRequest request, string message)
        {

            var result = new Response<PaymentWSResponse>();

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

            await this._transactionRepository.AddAsync(transaction);
            await this._logTransactionRepository.AddAsync(logTransaction);
            await this._unitOfWork.SaveChangesAsync();
            
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
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// VALIDACIÓN DE REGLAS PIORPI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<List<ValidacionPrp>> ValidatePIORPI(string name, decimal total)
        {
            var response      = new List<ValidacionPrp>();
            var starMonthtDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastMonthDay  = (new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(1).Month, 1).AddDays(-1));
            var monday        = this.FirstDateInWeek(DateTime.Today);
            var sunday        = this.LastDayOfWeek(DateTime.Today);

            var transactionsMonth = _transactionRepository.GetMany(t => t.NameCreditCard == name && (t.CreateDate >= starMonthtDay && t.CreateDate <= lastMonthDay) && t.AutorizationId != null).ToList();
            var transactionsDay   = transactionsMonth.Where(t => t.CreateDate.Date == DateTime.Today.Date).ToList();
            var trasactionsWeek   = transactionsMonth.Where(t => t.CreateDate.Date >= monday.Date && t.CreateDate.Date <= sunday.Date).ToList();

            var limitDay   = decimal.Parse(ConfigurationManager.AppSettings["PIORPI.Limit.Day"].ToString());
            var limitWeek  = decimal.Parse(ConfigurationManager.AppSettings["PIORPI.Limit.Week"].ToString());
            var limitYear  = decimal.Parse(ConfigurationManager.AppSettings["PIORPI.Limit.Month"].ToString());
            var totalDia   = transactionsDay.Sum(t => t.Amount) + total;
            var totalWeek  = trasactionsWeek.Sum(t => t.Amount) + total;
            var totakMonth = transactionsMonth.Sum(t => t.Amount) + total;

            // Al solicitar una o más transacciones que sumen 40 mil pesos o mayor en un día, se requiere una validación con el cliente.
            if (totalDia >= limitDay )
            {
                response.Add(new ValidacionPrp($"Limite díario de {limitDay.ToString("C")} superado, se requiere aprobación por parte del cliente", EnumReglaPiorpi.LimiteExcedidoMontoDiario));
            }

            // Al solicitar más de 2 transacciones en un solo día, se requiere una validación con el cliente.
            if (transactionsDay.Count >= 2)
            {
                response.Add(new ValidacionPrp("Se supero el número de compras diarios, se requiere aprobación por parte del cliente", EnumReglaPiorpi.LimiteComprasPorDia));
            }

            // Al solicitar transacciones que sumen 80 mil pesos o mas en una semana, se requiere una validación con el cliente.
            if ( totalWeek >= limitWeek )
            {
                response.Add(new ValidacionPrp($"Se supero el limite de compras semanal de {limitWeek.ToString("C")}, se requiere aprobación por parte del cliente", EnumReglaPiorpi.LimiteExcedidoMontoSemanal));
            }

            // Al solicitar una o más transacciones que sumen 100 mil pesos o mas en un mes, se requiere una validación con el cliente.
            if (totakMonth  >= limitYear)
            {
                response.Add(new ValidacionPrp($"Se supero el limite de compras mensual de {limitYear.ToString("C")}, se requiere aprobación por parte del cliente", EnumReglaPiorpi.LimiteExcedidoMontoMensual));
            }

            return response;
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