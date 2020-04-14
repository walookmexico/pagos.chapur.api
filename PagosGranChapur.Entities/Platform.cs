using PagosGranChapur.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Entities
{
    public class Platform: IEntityBase
    {
        // IDENTIFICADOR DE LA PLATAFORMA
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        // DESCRIPCIÓN DE LA PLATAFORMA
        public string Description { get; set; }
    }
}
