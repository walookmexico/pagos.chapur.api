using PagosGranChapur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
