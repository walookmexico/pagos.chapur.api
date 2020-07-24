using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.WebServerRequest;

namespace PagosGranChapur.Services.Helpers
{
    public class Converter
    {

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
    }
}
