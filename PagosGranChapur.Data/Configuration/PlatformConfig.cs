using PagosGranChapur.Entities;

namespace PagosGranChapur.Data.Configuration
{
    public class PlatformConfig: EntityBaseConfiguration<Platform>
    {
        public PlatformConfig() {

            Property(p => p.Description)
                .IsRequired();

            ToTable("plataformas");

            Property(p => p.Id).HasColumnName("plataformaId")
                               .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None); 

            Property(p => p.Description).HasColumnName("descripcion");
        }
    }
}
