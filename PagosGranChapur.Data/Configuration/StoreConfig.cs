﻿using PagosGranChapur.Entities;

namespace PagosGranChapur.Data.Configuration
{
    public class StoreConfig: EntityBaseConfiguration<Store>
    {
        public StoreConfig() {
            Property(s => s.Description)                
                .IsRequired();

            ToTable("tiendas");

            Property(s => s.Id).HasColumnName("tiendaId")
                               .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(s => s.Description).HasColumnName("descripcion");
        
        }
    }
}
