using PagosGranChapur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Data.Configuration
{
    public class UserConfig : EntityBaseConfiguration<User>
    {
        public UserConfig() {

            Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(500);

            Property(u => u.UserName)
                 .IsRequired()
                 .HasMaxLength(50);

            Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(null);

            Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(300);

            Property(u => u.RolId)
                .IsRequired();

            Property(u => u.PasswordTemp)
                .IsOptional();

            Property(u => u.ChangePassword)
                .HasColumnType("bit")
                .IsRequired();

            Property(u => u.CreateDate)
                .IsRequired();

            Property(u => u.UpdateDate)
                .IsRequired();

            ToTable("usuarios");

            Property(u => u.Id).HasColumnName("usuarioId");
            Property(u => u.UserName).HasColumnName("nombreUsuario");
            Property(u => u.Password).HasColumnName("contrasenia");
            Property(u => u.Email).HasColumnName("correoElectronico");
            Property(u => u.RolId).HasColumnName("rolId");
            Property(u => u.PasswordTemp).HasColumnName("contraseniaTemp");
            Property(u => u.ChangePassword).HasColumnName("cambiarContrasenia");
            Property(u => u.CreateDate).HasColumnName("fechaCreacion");
            Property(u => u.UpdateDate).HasColumnName("fechaActualizacion");


        }
    }
}
