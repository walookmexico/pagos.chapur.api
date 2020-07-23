using PagosGranChapur.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

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
