using PagosGranChapur.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace PagosGranChapur.Data.Configuration
{
    public class TransactionConfig : EntityBaseConfiguration<Transaction>
    {
        public TransactionConfig() {

            Property(t => t.CreateDate)
               .IsOptional();

            Property(t => t.Amount)
               .IsOptional();

            Property(t => t.NameCreditCard)
                .IsOptional()
                .HasMaxLength(500);

            Property(t => t.AutorizationId)    
                .IsUnicode()
                .IsOptional();

            Property(t => t.StoreId)
              .IsOptional();

            Property(t => t.StoreDescripcion)
                .IsOptional()
                .HasMaxLength(500);

            Property(t => t.PlatformId)
                .IsOptional();

            Property(t => t.DecriptionPlatform)               
               .HasMaxLength(250);

            Property(t => t.UserId)
               .IsOptional();

            Property(t => t.PurchaseOrderId)
                .IsOptional();

            Property(t => t.Email)
                .IsOptional()
                .HasMaxLength(500);

           Property(t => t.CashOperationId)
                 .IsOptional();

            Property(t => t.ValidationDate)
                  .IsOptional();

            Property(t => t.StatusPiorpi)
                  .IsRequired();

            Property(t => t.DetailPiorpi)
                  .IsOptional();

            Property(t => t.StatusTransaction)
                  .IsOptional();

            Property(t => t.Months)
               .IsRequired();
            
            Property(t => t.WithInterest)
            .IsRequired();
            
            // Table & Column Mappings
            ToTable("transacciones");
            Property(t => t.Id).HasColumnName("transaccionId");
            Property(t => t.CreateDate).HasColumnName("fechaHoraAplicacion");
            Property(t => t.Amount).HasColumnName("total");
            Property(t => t.NameCreditCard).HasColumnName("cuentahabienteNombre");
            Property(t => t.AutorizationId).HasColumnName("autorizacionId");
            Property(t => t.StoreId).HasColumnName("tiendaId");
            Property(t => t.StoreDescripcion).HasColumnName("tiendaDescripcion");
            Property(t => t.PlatformId).HasColumnName("plataformaId");
            Property(t => t.DecriptionPlatform).HasColumnName("descripcionPlataforma");
            Property(t => t.UserId).HasColumnName("usuarioId");
            Property(t => t.PurchaseOrderId).HasColumnName("ordenCompraId");            
            Property(t => t.Email).HasColumnName("correoElectronicoUsuario");
            Property(t => t.CashOperationId).HasColumnName("operacionCajaId");
            Property(t => t.ValidationDate).HasColumnName("fechaValidacion");
            Property(t => t.StatusPiorpi).HasColumnName("estatusPiorpi");
            Property(t => t.DetailPiorpi).HasColumnName("detallePiorpi");
            Property(t => t.StatusTransaction).HasColumnName("estatusTransaccion");
            Property(t => t.Months).HasColumnName("meses");
            Property(t => t.WithInterest).HasColumnName("conIntereses");
        }

    }
}
