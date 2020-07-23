using PagosGranChapur.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

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
