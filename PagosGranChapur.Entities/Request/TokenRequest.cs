using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities.Request
{
    public class TokenRequest
    {
        // IDENTIFICADOR DE LA TIENDA 
        [Required]
        [Range(1,int.MaxValue, ErrorMessage ="El valor del identificador de la tienda debe ser mayor {1}")]
        public int StoreId          { get; set; }

        // NUMERO DE LA TARJETA DE CRÉDITO
        [Required]
        public string NoCreditCard  { get; set; }

        // IDENTIFICADOR DE LA ACCION A REALIZAR
        [Required]
        [Range(1,int.MaxValue, ErrorMessage ="El valor del identificador de la acción debe ser mayor {1}")]
        public int Action           { get; set; }

        // CORREO ELECTRONICO DEL CLIENTE
        [Required]        
        public string Email { get; set; }
    }
}
