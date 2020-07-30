using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Enums;
using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Repositories;
using PagosGranChapur.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Services
{
    public interface IUserService
    {
        Task<Response<UserApplication>> ValidateUser(string username, string password);
        Task<Response<List<User>>> GetAll();
        Task<Response<User>> GetById(int userId);
        Task<Response<User>> AddUser(SaveUserRequest request);
        Task<Response<User>> UpdateUser(UpdateUserRequest request);
        Task<Response<bool>> DeleteUser(int userId);
        Task<Response<User>> RecoveryPassword(string email, string bodyHTML);
        Task<Response<bool>> UpdatePasword(int userId, UpdatePasswordRequest request);
    }

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public UserService(IUnitOfWork unitOfWork, IUserRepository userRepository ) {

            _unitOfWork     = unitOfWork;
            _userRepository = userRepository;
        }

        #region INTERFACE METHODS
        /// <summary>
        /// Validar usuario y contraseña
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Response<UserApplication>> ValidateUser(string username, string password)
        {
            Response<UserApplication> userResponse = await Task.Run(() =>
            {
                var response = new Response<UserApplication>();

                try
                {
                    var encryptPassword = this.EncodeMD5(password);
                    var user = _userRepository.Get(x => x.UserName == username && x.Password == encryptPassword);

                    if (user == null)
                        throw new PagosChapurException("No se encontró usuario con los datos proporcionados");


                    response.IsSuccess = true;
                    response.Data = new UserApplication
                    {
                        EmailAddress = user.Email,
                        Id = user.Id,
                        FullName = user.FullName,
                        Rol = user.RolId == 1 ? EnumRoles.Administrator : EnumRoles.Consulting,
                        UserName = user.UserName,
                        ChangePassword = user.ChangePassword,
                    };
                }
                catch (Exception ex)
                {
                    ResponseConverter.SetErrorResponse(response, "Problemas al validar el usuario: ");
                    response.InternalError = ex.Message;

                }

                return response;
            });

            return userResponse;
        }

        /// <summary>
        /// Agregar un nuevo usuario en la base de datos 
        /// </summary> 
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<User>> AddUser(SaveUserRequest request)
        {
            Response<User> addUserResponse = await Task.Run(() =>
            {
                var response = new Response<User>();

                try
                {
                    var user = (new User
                    {
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Email = request.Email.Trim().ToLower(),
                        FullName = request.FullName,
                        Password = this.EncodeMD5(request.Password),
                        RolId = request.RolId,
                        UserName = request.UserName.Trim().ToLower()
                    });

                    _userRepository.Add(user);
                    _unitOfWork.Commit();

                    response.Data = user;
                    ResponseConverter.SetSuccessResponse(response, "Usuario creado correctamente");
                }
                catch (Exception ex)
                {
                    ResponseConverter.SetErrorResponse(response, "Problemas al validar el usuario");
                    response.InternalError = ex.Message;
                }

                return response;
            });

            return addUserResponse;
        }

        /// <summary>
        /// Actualizar los datos de un usuario
        /// </summary>
        /// <param name="request">Datos del usuario</param>
        /// <returns>Objeto User con los datos actualizados</returns>
        public async Task<Response<User>> UpdateUser(UpdateUserRequest request)
        {
            Response<User> updateUserResponse = await Task.Run(() =>
            {
                var response = new Response<User>();

                try
                {
                    var user = _userRepository.GetById(request.Id);

                    if (user == null) 
                        throw new PagosChapurException("Usuario no encontrado dentro de la based de datos");

                    user.FullName = request.FullName;
                    user.RolId = request.RolId;
                    user.UserName = request.UserName;
                    user.UpdateDate = DateTime.Now;
                    user.Email = request.Email;

                    _userRepository.Update(user);
                    _unitOfWork.Commit();

                    response.Data = user;
                    ResponseConverter.SetSuccessResponse(response, "Datos del usuario actualizados correctamente");
                }
                catch (Exception ex)
                {
                    ResponseConverter.SetErrorResponse(response, "Problemas al actualizar los datos del usuario");
                    response.InternalError = ex.Message;
                }

                return response;
            });

            return updateUserResponse;
        }
                
        /// <summary>
        /// Eliminar el registro físico de la base de datos de un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public async Task<Response<bool>> DeleteUser(int userId) {

            var response = new Response<bool>();

            try
            {
                var user = _userRepository.GetById(userId);

                if (user != null)
                {
                    await _userRepository.DeleteAsync(user);
                    _unitOfWork.Commit();

                    response.Data = true;
                    ResponseConverter.SetSuccessResponse(response, "Usuario eliminado correctamente de la base de datos");
                }
                else {
                    response.Data = false;
                    ResponseConverter.SetErrorResponse(response, "Datos de usuario no encontrados");                    
                }
                
            }
            catch (Exception ex )
            {
                ResponseConverter.SetErrorResponse(response, "Error al intentar eliminar el usuario");
                response.InternalError = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Obtiene todos los usuarios registrados
        /// </summary>
        /// <returns>Listado de usuarios</returns>
        public async Task<Response<List<User>>> GetAll()
        {
            var response = new Response<List<User>>();

            try
            {
                response.Data      = (await _userRepository.GetAllAsync()).ToList();
                ResponseConverter.SetSuccessResponse(response, "Listado de usuario obtenido d emanera exitosa");
            }
            catch (Exception)
            {
                ResponseConverter.SetErrorResponse(response, "Error al obtener el listado de usuarios");
                response.Data      = null;
            }

            return response;
            
        }

        /// <summary>
        /// Obtiene los datos de un usuario dado un identificador
        /// </summary>
        /// <param name="userId">Identificador principal del usuario</param>
        /// <returns>Objeto tipo User que contiene todos los datos del usuario</returns>
        public async Task<Response<User>> GetById(int userId)
        {
            Response<User> userByIdResponse = await Task.Run(() => 
            { 
                var response = new Response<User>();

                try
                {
                    response.Data = _userRepository.GetById(userId);
                    ResponseConverter.SetSuccessResponse(response, "Listado de usuario obtenido d emanera exitosa");
                }
                catch (Exception)
                {
                    ResponseConverter.SetErrorResponse(response, "Error al obtener el listado de usuarios");
                    response.Data = null;
                }

                return response;
            });

            return userByIdResponse;
        }

        /// <summary>
        ///  Método que permite generar un password temporal
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Response<User>> RecoveryPassword(string email, string bodyHTML) {

            Response<User> recoveryPasswordResponse = await Task.Run(() =>
            {
                var response = new Response<User>();

                try
                {
                    var user = _userRepository.Get(x => x.Email.Trim() == email.Trim());

                    if (user == null)
                    {
                        ResponseConverter.SetErrorResponse(response, $"El usuario con correo eléctronico { email } no ha sido encontrado, favor de corroborar la información");
                        return response;
                    }

                    var password = RandomString(18);
                    user.Password = EncodeMD5(password);
                    user.UpdateDate = DateTime.Now;
                    user.Email = user.Email.Trim();
                    user.UserName = user.UserName.Trim();
                    user.ChangePassword = true;

                    _userRepository.Update(user);
                    _unitOfWork.SaveChanges();

                    bodyHTML = bodyHTML.Replace("[token]", password);
                    MailService.SendMessage(email, bodyHTML, "Password");

                    ResponseConverter.SetSuccessResponse(response, "Se han generado una contraseña temporal la cuál se mando al correo proporcionado ");
                }
                catch (Exception ex)
                {
                    response.Data = null;
                    ResponseConverter.SetErrorResponse(response, ex.Message);
                }

                return response;
            });

            return recoveryPasswordResponse;
        }

        /// <summary>
        /// Método para actualizar el nuevo password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Response<bool>> UpdatePasword(int userId, UpdatePasswordRequest request) {

            var response = new Response<bool>();

            try
            {
                var encrypLastPassword = EncodeMD5(request.LastPassword); 
                var user = await _userRepository.FindAsync(x => x.Id == userId && x.Password == encrypLastPassword);

                if (user == null)
                {
                    ResponseConverter.SetErrorResponse(response, $"El usuario no ha sido encontrado, favor de corroborar la información");
                    return response;
                }
                                
                user.Password       = EncodeMD5(request.NewPassword);
                user.UpdateDate     = DateTime.Now;
                user.ChangePassword = false;

                _userRepository.Update(user);
                _unitOfWork.SaveChanges();

                ResponseConverter.SetSuccessResponse(response, "Se actualizó correctamente la contraseña del usuario");
            }
            catch (Exception ex)
            {
                response.Data       = false;
                ResponseConverter.SetErrorResponse(response, ex.Message);
            }

            return response;


        }

        #endregion

        #region PRIVATE METHODS 

        /// <summary>
        /// Convertir una cadena en MD5
        /// </summary>
        /// <param name="passowd"></param>
        /// <returns></returns>
        private string EncodeMD5(string password)
        {
            byte[] originalBytes;
            byte[] encodedBytes;
            MD5 md5;

            md5 = new MD5CryptoServiceProvider();
            originalBytes = Encoding.Default.GetBytes(password);
            encodedBytes = md5.ComputeHash(originalBytes);

            return BitConverter.ToString(encodedBytes);
        }

        /// <summary>
        /// Create password ramdom
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RandomString(int length)
        {
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(chars[(int)(num % (uint)chars.Length)]);
                }
            }

            return res.ToString();
        }

        #endregion

    }
}
