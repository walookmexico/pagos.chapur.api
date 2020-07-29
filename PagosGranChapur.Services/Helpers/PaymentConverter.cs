using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.WebServerRequest;
using PagosGranChapur.Entities.WebServerResponses;
using System;
using System.Collections.Generic;

namespace PagosGranChapur.Services.Helpers
{
    public class PaymentConverter
    {
        private PaymentConverter() { }

        public static OrdenCompraRequest OrderPaymentToOrdenCompra(SaveOrderPaymentRequest request)
        {
            return new OrdenCompraRequest
            {
                CorreoElectronico = request.Email,
                CVV = request.CVV,
                FechaAlta = request.CreateDate,
                IdOrden = request.IdPurchaseOrder,
                IdPlataforma = request.PlatformId,
                IdTienda = request.IdStore,
                Monto = (float)request.Amount,
                NombreCuentaHabiente = request.NameCreditCard,
                NoTarjeta = request.NoCreditCard,
                PasswordPlataforma = request.PasswordPlatform,
                Token = request.Token,
                UsuarioPlataforma = request.UserPlatform,
                Meses = request.Months.Value,
                ConInteres = request.WithInterest.Value
            };
        }

        public static OrdenCompraDetalleRequest SaveDetailPaymentToOrdenCompraDetalle(SaveDetailPaymentRequest request)
        {
          
            return new OrdenCompraDetalleRequest
            {
                CorreoElectronico = request.Email,
                CVV = request.CVV,
                FechaAlta = request.CreateDate,
                IdOrden = request.IdPurchaseOrder,
                IdPlataforma = request.PlatformId,
                IdTienda = request.IdStore,
                Monto = (float)request.Amount,
                NombreCuentaHabiente = request.NameCreditCard,
                NoTarjeta = request.NoCreditCard,
                PasswordPlataforma = request.PasswordPlatform,
                Token = request.Token,
                UsuarioPlataforma = request.UserPlatform,
                Meses = request.Months.Value,
                ConInteres = request.WithInterest.Value,
                ListadoDetalle = ToListDetalleCompra(request)
            };
            
        }

        public static List<DetalleCompra> ToListDetalleCompra(SaveDetailPaymentRequest request)
        {
            var details = new List<DetalleCompra>();

            request.Detail.ForEach(detailOrden =>
            {
                details.Add(new DetalleCompra
                {
                    Cantidad = detailOrden.Quantity,
                    CostoPromedio = detailOrden.AverageCost,
                    Descuento = detailOrden.Discount,
                    IdArticulo = detailOrden.ItemId,
                    IEPS = detailOrden.IEPS,
                    IVA = detailOrden.IVA,
                    PrecioSinImpuesto = detailOrden.PriceWithoutTax
                });
            });

            return details;
        }

        public static Transaction ToTransaction(PaymentWSResponse response, SavePaymentRequest request)
        {
            return new Transaction
            {
                DecriptionPlatform = response.DescripcionPlataforma,
                NameCreditCard = request.NameCreditCard,
                StoreDescripcion = response.DescripcionTienda,
                UserId = response.IdUsuario != null ? response.IdUsuario.Value : 0,
                PlatformId = response.IdPlatadorma != null ? response.IdPlatadorma.Value : 0,
                AutorizationId = response.IdAutorizacion != null ? response.IdAutorizacion.ToString() : "0",
                Amount = response.Monto != null ? decimal.Parse(response.Monto.Value.ToString()) : 0,
                CreateDate = response.FechaHoraAplicacion != null ? response.FechaHoraAplicacion.Value : DateTime.Now,
                PurchaseOrderId = request.IdPurchaseOrder,
                StoreId = response.IdTienda != null ? response.IdTienda.Value : 0,
                Email = request.Email,
                Months = request.Months.Value,
                WithInterest = request.WithInterest.Value
            };
        }

        public static LogTransaction ToLogTransaction(Transaction transaction, PaymentWSResponse response)
        {
            return new LogTransaction
            {
                DateApply = transaction.CreateDate,
                MessageResponse = response.Mensaje,
                PurchaseOrderID = transaction.PurchaseOrderId,
                Status = response.Estatus != null ? response.Estatus.Value : 0,
                TransactionId = transaction.Id,
                PlatformId = transaction.PlatformId
            };
        }

        public static PaymentWSResponse LogTransactionToPaymentResponse(LogTransaction logTransaction, Transaction transaction)
        {
            return new PaymentWSResponse
            {
                Estatus = logTransaction.Status,
                IdAutorizacion = transaction.AutorizationId,
                IdPlatadorma = transaction.PlatformId,
                Mensaje = logTransaction.MessageResponse,
                FechaHoraAplicacion = transaction.CreateDate,
                Monto = (float)transaction.Amount,
                NombreCuentaHabiente = transaction.NameCreditCard,
                DescripcionTienda = transaction.StoreDescripcion,
                DescripcionPlataforma = transaction.DecriptionPlatform
            };
        }
    }
}
