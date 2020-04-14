using Newtonsoft.Json;
using PagosGranChapur.Entities.Enums;
using PagosGranChapur.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities.WebServerRequest
{
    public class OrdenCompraRequestBase {

        // IDENTIFICADOR DE LA PLATAFORMA
        [JsonProperty(PropertyName = "idPlataforma")]
        public int IdPlataforma { get; set; }

        // USUARIO DE LA PLATAFORMA PROPORCIONADO POR CHAPUR
        [JsonProperty(PropertyName = "usuarioPlataforma")]
        public string UsuarioPlataforma { get; set; }

        // CONTRASEÑA DE LA PLATAFORMA PROPORCIONADO POR CHAPUR
        [JsonProperty(PropertyName = "passwordPlataforma")]
        public string PasswordPlataforma { get; set; }

        // NUMERO DE TARJETA
        [JsonProperty(PropertyName = "noTarjeta")]
        public string NoTarjeta { get; set; }

        // FECHA DE ALTA DE LA TARJETA DE CRÉDITO
        [JsonProperty(PropertyName = "fechaAlta")]
        public string FechaAlta { get; set; }

        // CODIGO CVV DE LA TARJETA DE CRÉDITO
        [JsonProperty(PropertyName = "cvv")]
        public string CVV { get; set; }

        // TOKEN DE CONFIRMACIÓN DE IDENTIDAD DEL TITULAR
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        // MONTO TOTAL DE LA OPERACION
        [JsonProperty(PropertyName = "monto")]
        public float Monto { get; set; }

        // IDENTIFICADOR DE LA TIENDA
        [JsonProperty(PropertyName = "idTienda")]
        public int IdTienda { get; set; }

        // NOMBRE DEL TITULAR DE LA TARJETA DE CRÉDITO
        [JsonProperty(PropertyName = "nombreCuentaHabiente")]
        public string NombreCuentaHabiente { get; set; }

        // CORREO ELECTRÓNICO DEL TITULAR
        [JsonProperty(PropertyName = "correoElectronico")]
        public string CorreoElectronico { get; set; }

        // IDENTIFICADOR DE LA ORDEN DE COMPRA
        [JsonProperty(PropertyName = "idOrden")]
        public string IdOrden { get; set; }

        // Determina el tipo validación de PIORPI se esta realizando
        [JsonProperty(PropertyName = "reglaPrp")]
        public int ReglaPrp { get; set; }

        // Números de meses en los que se aplicará la compra
        [JsonProperty(PropertyName = "meses")]
        public int Meses { get; set; }

        // Si la intereses en la compra
        [JsonProperty(PropertyName = "conInteres")]
        public int ConInteres { get; set; }

        [JsonProperty(PropertyName = "detalleValidacionesPrp")]
        public List<ValidacionPrp> DetalleValidacionesPrp { get; set; } = new List<ValidacionPrp>();

    }

    public class OrdenCompraRequest: OrdenCompraRequestBase { }

    public class OrdenCompraDetalleRequest: OrdenCompraRequestBase
    {      
        // LISTADO DE ARTICULOS QUE INTEGRAN LA COMPRA
        [JsonProperty(PropertyName = "listadoDetalle")]
        public List<DetalleCompra> ListadoDetalle { get; set; }             
    }

    public class DetalleCompra {

        // IDENTIFICADOR DEL PRODUCTO EN LA PLATAFORMA
        [JsonProperty(PropertyName = "idArticulo ")]
        public int IdArticulo { get; set; }

        // PRECIO SIN IMPUETOS
        [JsonProperty(PropertyName = "precioSinImpuesto")]
        public float PrecioSinImpuesto { get; set; }

        // IMPUESTO AL VALOR AGREGADO 
        [JsonProperty(PropertyName = "iva")]
        public float IVA { get; set; }

        // IMPUESTO ESPECIAL SOBRE PRODUCTOS Y SERVICIOS
        [JsonProperty(PropertyName = "ieps")]
        public float IEPS { get; set; }

        // COSTO SIN IMPUESTOS
        [JsonProperty(PropertyName = "costoPromedio")]
        public float CostoPromedio { get; set; }

        // NÚMERO DE ARTICULOS 
        [JsonProperty(PropertyName = "cantidad")]
        public int Cantidad { get; set; }

        // CANTIDAD QUE SE DESCONTO DEL TOTAL DEL PRODUCTO
        [JsonProperty(PropertyName = "descuento")]
        public float Descuento { get; set; }
    }

    public class ValidacionPrp {

        public ValidacionPrp(string descripcion, EnumReglaPiorpi tipo)
        {
            this.Descripcion = descripcion;
            this.ReglaPrp    = (int)tipo;
        }

        [JsonProperty(PropertyName = "descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty(PropertyName = "reglaPrp")]
        public int ReglaPrp       { get; set; }
    }
}
