using PagosGranChapur.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PagosGranChapur.Entities
{
    public class Transaction : IEntityBase
    {

        public Transaction() {
            this.WithInterest = 0;
            this.Months       = 0;
        }

        // IDENTIFICADOR
        public int Id                           { get; set; }

        // FECHA Y HORA DE RESPUESTA
        public DateTime CreateDate              { get; set; }

        // MONTO DE LA TRANSACCIÓN
        public decimal Amount                   { get; set; }

        // NOMBRE COMO APARECE EN LA TARJETA
        public string NameCreditCard            { get; set; }

        // CUENTA DE CORREO DEL USUARIO QUE REALIZA LA COMPRA
        public string Email                     { get; set; }

        // IDENTIFICADOR AUTORIZACIÓN
        public string AutorizationId            { get; set; }

        // IDENTIFICADOR DE LA TIENDA
        public int StoreId                      { get; set; }

        // DESCIPCION DE LA TIENDA 
        public string StoreDescripcion          { get; set; }

        // IDENTIFICADOR DE LA PLATAFORMA
        public int PlatformId                   { get; set; }

        // DESCRIPCION DE LA PLATAFORMA
        public string DecriptionPlatform        { get; set; }

        // IDENTIFICADOR DEL USUARIO
        public int UserId                       { get; set; }

        // ID ORDEN DE COMPRA
        public string PurchaseOrderId           { get; set; }
        
        // OPERACIÓN DE CAJA
        public int? CashOperationId             { get; set; }
        
        // OPERACIÓN DE CAJA
        public DateTime? ValidationDate         { get; set; }

        // Estatus de la transacción PIORPI
        public int StatusPiorpi                 { get; set; }

        // Detail status PIORPI
        public string DetailPiorpi              { get; set; }

        // Estatus de la transacción en CHAPUR
        public int? StatusTransaction           { get; set; }

        // Determina si son meses con intereses
        public int Months                       { get; set; }

        // Aplica intereses en la compra
        public int WithInterest                { get; set; }


    }
}
