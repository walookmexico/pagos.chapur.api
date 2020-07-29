using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Enums;
using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Entities.WebServerRequest;
using PagosGranChapur.Entities.WebServerResponses;
using PagosGranChapur.Repositories;
using PagosGranChapur.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagosGranChapur.Services
{
    public interface ITransactionService
    {
        Task<Response<bool>> LoadData();
        Task<Response<List<Transaction>>> Filter(DateTime stardDate, DateTime? endDate, int[] storeId, int[] platformId);
        Task<Response<List<LogTransaction>>> FilterLog(DateTime stardDate, DateTime? endDate, int[] storeId, int[] platformId);
        Task<Response<List<Transaction>>> CheckStatusTransactions(DateTime stardDate, DateTime? endDate, string apiUrl, string baseURL);
        Task<Response<List<Transaction>>> CheckStatusTransactionsWithOutId(string apiUrl, string baseURL);
    }
    
    public class TransactionService: ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogTransactionRepository _logTransactionRepository;


        public TransactionService(
            ITransactionRepository transactionRepository,
            ILogTransactionRepository logTransactionRepository,
            IUnitOfWork unitOfWork) {

            _transactionRepository    = transactionRepository;
            _logTransactionRepository = logTransactionRepository;
            _unitOfWork               = unitOfWork;
        }

        #region INTERFACE METHODS

        /// <summary>
        /// Obtiene todos los datos de la tabla de transacciones filtrada por los parametros nviados
        /// </summary>
        /// <param name="stardDate"></param>
        /// <param name="endDate"></param>
        /// <param name="storeId"></param>
        /// <param name="platformId"></param>
        /// <returns></returns>
        public async Task<Response<List<Transaction>>> Filter(DateTime stardDate, DateTime? endDate, int[] storeId, int[] platformId)
        {
            Response<List<Transaction>> filterResponse = await Task.Run(() =>
            {
                var response = new Response<List<Transaction>>();

                try
                {
                    var start = stardDate.Date;
                    var end = endDate.Value.Date.AddDays(1);

                    var data = _transactionRepository.GetMany(t => t.CreateDate >= start
                                                                            && t.CreateDate <= end);

                    if (platformId.Where(x => x > 0).Count() > 0)
                        data = data.Where(t => platformId.Contains(t.PlatformId)).ToList();

                    if (storeId.Where(x => x > 0).Count() > 0)
                        data = data.Where(t => storeId.Contains(t.StoreId)).ToList();


                    response.Data = data?.ToList();
                    ResponseConverter.SetSuccessResponse(response, "Datos obtenidos correctamente");

                }
                catch (Exception ex)
                {
                    response.InternalError = ex.Message;
                    ResponseConverter.SetErrorResponse(response, "Error al filtar los datos");
                }

                return response;
            });

            return filterResponse;
        }

        /// <summary>
        /// FILTRAR LAS PETICIONES DEL LOG POR FECHA Y PLATAFORMA
        /// </summary>
        /// <param name="stardDate"></param>
        /// <param name="endDate"></param>
        /// <param name="storeId"></param>
        /// <param name="platformId"></param>
        /// <returns></returns>
        public async Task<Response<List<LogTransaction>>> FilterLog(DateTime stardDate, DateTime? endDate, int[] storeId, int[] platformId)
        {
            var response = new Response<List<LogTransaction>>();

            try
            {
                var start = stardDate.Date;
                var end = endDate.Value.Date.AddDays(1);

                var data = await _logTransactionRepository.FindAllAsync(t => t.DateApply >= start && t.DateApply <= end);

                if (platformId.Where(x => x > 0).Count() > 0)                
                    data = data.Where(t => platformId.Contains(t.PlatformId)).ToList();

                response.Data      = data?.ToList();
                ResponseConverter.SetSuccessResponse(response, "Datos obtenidos correctamente");
            }
            catch (Exception ex)
            {
                response.InternalError  = ex.Message;
                ResponseConverter.SetErrorResponse(response, "Error al filtar los datos");
            }

            return response;
        }
        
        /// <summary>
        /// VALIDAR EL ESTATUS DE LA OPERACION DE CAJA POR UNA FECHA DETERMINADA
        /// </summary>
        /// <param name="stardDate"></param>
        /// <param name="endDate"></param>
        /// <param name="apiUrl"></param>
        /// <param name="baseURL"></param>
        /// <returns></returns>
        public async Task<Response<List<Transaction>>> CheckStatusTransactions(DateTime stardDate, DateTime? endDate, string apiUrl, string baseURL) {
            
            var response = new Response<List<Transaction>>();
            
            try
            {
                var data =  _transactionRepository.GetAll()
                                                       .ToList()
                                                       .Where(t => (t.CreateDate.Date >= stardDate && t.CreateDate.Date <= endDate)
                                                                    && (t.CashOperationId == null || t.CashOperationId == 0)
                                                                    && t.AutorizationId != null);
                                                                         

                foreach (var t in data) {

                    var request = new EstatusCompraWSRequest
                    {
                        IdAutorizacion = int.Parse(t.AutorizationId.ToString()),
                        IdTienda       = t.StoreId
                    };

                    var statusPurchaseOrder = await RequestService.Post<EstatusCompraWSRequest, EstatusCompraWSResponse>(apiUrl, baseURL, request);

                    t.StatusTransaction = statusPurchaseOrder.Estatus;

                    if (statusPurchaseOrder.IdOperacionCaja > 0)
                    {
                        t.CashOperationId   = statusPurchaseOrder.IdOperacionCaja;
                        t.ValidationDate    = DateTime.Now;                       

                        if(statusPurchaseOrder.Estatus == 1)
                            t.StatusPiorpi    = (int)EnumPiorpi.Correcto;
                    }
                    else
                    {
                        t.ValidationDate = DateTime.Now;
                    }

                    _transactionRepository.Update(t);
                }

                _unitOfWork.SaveChanges();

                response.Data      = data?.ToList();
                ResponseConverter.SetSuccessResponse(response, "Datos obtenidos correctaente");
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                response.InternalError = httpEx.Message;
                ResponseConverter.SetErrorResponse(response, "Error al conectarse con los servicios de Chapur");
            }
            catch (Exception ex)
            {
                response.InternalError = ex.Message;
                ResponseConverter.SetErrorResponse(response, "Error al filtar los datos");
            }

            return response;

        }

        /// <summary>
        /// OPTENER LOS ID'S DE LAS OPERACIONES DE CAJA DE LAS TRANSACCIONES QUE NO LO TENGAN
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="baseURL"></param>
        /// <returns></returns>
        public async Task<Response<List<Transaction>>> CheckStatusTransactionsWithOutId(string apiUrl, string baseURL)
        {

            var response = new Response<List<Transaction>>();
            
            try
            {
                var data = await _transactionRepository.FindAllAsync(t => (t.CashOperationId == null || t.CashOperationId == 0) && t.AutorizationId != null);

                foreach (var t in data)
                {
                    var request = new EstatusCompraWSRequest
                    {
                        IdAutorizacion = int.Parse(t.AutorizationId.ToString()),
                        IdTienda = t.StoreId
                    };

                    var statusPurchaseOrder = await RequestService.Post<EstatusCompraWSRequest, EstatusCompraWSResponse>(apiUrl, baseURL, request);

                    t.StatusTransaction = statusPurchaseOrder.Estatus;

                    if (statusPurchaseOrder.IdOperacionCaja > 0)
                    {
                        t.CashOperationId = statusPurchaseOrder.IdOperacionCaja;
                        t.ValidationDate = DateTime.Now;

                        if (statusPurchaseOrder.Estatus == 1)
                            t.StatusPiorpi = (int)EnumPiorpi.Correcto;
                    }
                    else
                    {
                        t.ValidationDate = DateTime.Now;
                    }

                    _transactionRepository.Update(t);
                }

                _unitOfWork.SaveChanges();

                response.Data = data?.ToList();
                ResponseConverter.SetSuccessResponse(response, "Datos obtenidos correctaente");
            }
            catch (Exception ex)
            {
                response.InternalError = ex.Message;
                ResponseConverter.SetErrorResponse(response, "Error al filtar los datos");
            }

            return response;

        }

        public async Task<Response<bool>> LoadData()
        {
            Response<bool> loadDataResponse = await Task.Run(() =>
            {
                var response = new Response<bool>();

                try
                {
                    var storeId = 1;
                    var sellByDay = 1;
                    var addDays = 1;
                    var descriptionStore = "E-COMMERCE";
                    var limitSellByDay = 10;

                    Random rnd = new Random();

                    for (int i = 1; i <= 4500; i++)
                    {


                        switch (storeId)
                        {
                            case 1:
                                descriptionStore = "E-COMMERCE";
                                break;
                            case 2:
                                descriptionStore = "VIAJES";
                                break;
                            default:
                                descriptionStore = "MESA DE REGALO";
                                break;

                        }

                        float division = i / 3F;
                        var isError = unchecked(division == (int)division);

                        var transaction = new Transaction
                        {
                            UserId = rnd.Next(1, 40000),
                            Amount = rnd.Next(350, 11000),
                            AutorizationId = rnd.Next(1, 20001).ToString(),
                            CreateDate = DateTime.Today.AddDays(addDays),
                            Email = "carga@integrait.com.mx",
                            NameCreditCard = "CARGA INICIAL",
                            //PurchaseOrderId = rnd.Next(1, 6000),
                            StoreId = rnd.Next(1, 3),
                            PlatformId = storeId,
                            DecriptionPlatform = descriptionStore,
                            StoreDescripcion = descriptionStore
                        };

                        if (storeId == 3)
                            storeId = 1;

                        if (sellByDay == limitSellByDay)
                        {
                            addDays++;
                            sellByDay = 1;
                            limitSellByDay = rnd.Next(5, 30);
                        }

                        sellByDay++;
                        storeId++;

                        _transactionRepository.Add(transaction);

                    }

                    _unitOfWork.Commit();
                    response.IsSuccess = true;
                    response.Data = true;
                }
                catch (Exception ex)
                {
                    ResponseConverter.SetErrorResponse(response, "Ocurrió un error al intentar realizar la carga inicial");
                    response.InternalError = ex.Message;
                }

                return response;
            });

            return loadDataResponse;
        }
        #endregion
    }
}
