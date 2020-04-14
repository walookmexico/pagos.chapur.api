using PagosGranChapur.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities
{
    public class LogTransaction: IEntityBase
    {
        // IDENTIFICADOR DEL LOG DE TRANSACCION
        public int Id                           { get; set; }

        [ForeignKey("Transaction")]
        // IDENTIFICADOR DE LA TRANSACCION
        public int? TransactionId                { get; set; }

        // NÚMERO DE ORDEN DE COMPRA 
        public string PurchaseOrderID           { get; set; }

        // FECHA DE APLICACION
        public DateTime? DateApply              { get; set; }

        // CODIGO DE ESTATUS DE LA TRANSACCIÓN ENVIADO POR CHAPUR
        public int Status                       { get; set; }

        // IDENTIFICADOR DE LA PLATAFORMA ORIGEN
        public int PlatformId                   { get; set; }

        // MENSAGE 
        public string MessageResponse           { get; set; }

        // RELACION CON LA TRANSACCION
        public virtual Transaction Transaction  { get; set; }
    }
}
