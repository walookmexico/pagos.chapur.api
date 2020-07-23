using PagosGranChapur.Entities.Base;
using System.Data.Entity.ModelConfiguration;

namespace PagosGranChapur.Data.Configuration
{
    public class EntityBaseConfiguration<T> : EntityTypeConfiguration<T> where T : class, IEntityBase
    {
        public EntityBaseConfiguration()
        {
            HasKey(e => e.Id);
        }
    }
}
