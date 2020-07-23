using System.ComponentModel.DataAnnotations;

namespace PagosGranChapur.Entities.Request
{
    public class SaveUserRequest
    {
        // IDENTIFICADOR DEL USUARIO
        public int? Id { get; set; }

        [Required]
        // NOMBRE COMPLETO DEL USUARIO
        public string FullName { get; set; }

        [Required]
        // NOMBRE DEL USUARIO PARA LA AUTENTICACION
        public string UserName { get; set; }

        [Required]
        // CONTRASEÑA DEL USUARIO LA CUALE STARA GUARDADA EN MD5
        public string Password { get; set; }

        [Required]
        // CORREO ELECTRONICO DEL USUARIO AL CUAL SE PUEDA MANDAR INFORMAIÓN
        // PARA REALIZAR SU RECUPERACIÓN DE CONTRASEÑA
        public string Email { get; set; }

        [Required]
        // IDENTIFICADOR DEL ROL 
        public int RolId { get; set; }

    }

    public class UpdateUserRequest
    {
        // IDENTIFICADOR DEL USUARIO
        [Required]
        public int Id { get; set; }

        [Required]
        // NOMBRE COMPLETO DEL USUARIO
        public string FullName { get; set; }

        [Required]
        // NOMBRE DEL USUARIO PARA LA AUTENTICACION
        public string UserName { get; set; }
               
        [Required]
        // CORREO ELECTRONICO DEL USUARIO AL CUAL SE PUEDA MANDAR INFORMAIÓN
        // PARA REALIZAR SU RECUPERACIÓN DE CONTRASEÑA
        public string Email { get; set; }

        [Required]
        // IDENTIFICADOR DEL ROL 
        public int RolId { get; set; }

    }

    public class TempPassowrdRequest {

        // CORREO ELECTRÓNICO DEL USUARIO
        [Required]
        public string Email { get; set; }
    }

    public class UpdatePasswordRequest {

        // CONTRASEÑA ACTUAL DEL USUARIO
        [Required]
        public string LastPassword { get; set; }

        // CONTRASEÑA NUEVA DEL USUARIO
        [Required]
        public string NewPassword { get; set; }
    }
}
