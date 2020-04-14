using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities
{
    public class CreditCard
    {
        // IDENTIFICADOR DE LA TARJETA DE CRÉDITO
        public int Id             { get; set; }

        // NUMNER DE TARJETA DE CRÉDITO
        public string Number      { get; set; }

        // FECHA DE APERTURA DE LA TARJETA DE CRÉDITO
        public string DateStart   { get; set; }

        // NOMBRE DEL CLIENTE
        public string Name        { get; set; }

        // CODIGO CVV DE LA TARJETA DE CRÉDITO
        public string CVV         { get; set; }

    }
}
