using PagosGranChapur.Entities.Base;
using System;

namespace PagosGranChapur.Entities
{
    public class User : IEntityBase
    {
        // IDENTIFICADOR DEL USUARIO
        public int Id               { get; set; }

        // NOMBRE COMPLETO DEL USUARIO
        public string FullName      { get; set; }

        // NOMBRE DEL USUARIO PARA LA AUTENTICACION
        public string UserName      { get; set; }

        // CONTRASEÑA DEL USUARIO LA CUALE STARA GUARDADA EN MD5
        public string Password      { get; set; }

        // CORREO ELECTRONICO DEL USUARIO AL CUAL SE PUEDA MANDAR INFORMAIÓN
        // PARA REALIZAR SU RECUPERACIÓN DE CONTRASEÑA
        public string Email         { get; set; }

        // IDENTIFICADOR DEL ROL 
        public int RolId            { get; set; }

        // CONTRASEÑA TEMPORAL ASIGNADA AL USUARIO EN CASO DE REALIZAR ESTE CAMBIO
        public string PasswordTemp  { get; set; }

        // BANDERA QUE PERMITIRÁ SABER SI EL USUARIO DEBE ACTUALIZAR LA CONTRASEÑA
        public bool ChangePassword  { get; set; }

        // FECHA EN QUE SE DIO DE ALTA AL USUARIO
        public DateTime CreateDate  { get; set; }

        // FECHA EN LA CUAL HUBO ALGUNA MODIFICACIÓN DE LOS DATOS DEL USUARIO
        public DateTime UpdateDate  { get; set; }
                
    }
}
