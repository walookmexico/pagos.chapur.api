using PagosGranChapur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Data.Configuration
{
    public class LogTransactionConfig : EntityBaseConfiguration<LogTransaction>
    {
        public LogTransactionConfig() {

            Property(t => t.TransactionId)
                .IsOptional();

            Property(t => t.PurchaseOrderID)
                .IsRequired();

            Property(t => t.MessageResponse)
                .IsRequired();

            Property(t => t.Status)
                .IsRequired();

            Property(t => t.PlatformId)
                .IsRequired();

            Property(t => t.DateApply)
                .IsRequired();
            
            ToTable("logtransacciones");

            Property(t => t.Id).HasColumnName("logTransaccionId");
            Property(t => t.TransactionId).HasColumnName("transaccionId");
            Property(t => t.PlatformId).HasColumnName("plataformaId");
            Property(t => t.PurchaseOrderID).HasColumnName("ordenCompra");
            Property(t => t.MessageResponse).HasColumnName("mensaje");
            Property(t => t.Status).HasColumnName("estatus");
            Property(t => t.DateApply).HasColumnName("fechaAplicacion");

        }
    }
}
