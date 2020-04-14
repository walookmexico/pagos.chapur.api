using PagosGranChapur.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities
{
    public  class Store: IEntityBase
    {
        // IDENTIFICADOR DE LA TIENDA
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id             { get; set; }

        // DSCRIPCIÓN DE LA TIENDA
        public string Description { get; set; }
    }
}
