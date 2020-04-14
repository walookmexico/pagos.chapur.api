using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Enums;
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

    public class UserService: IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

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
            var response = new Response<UserApplication>();

            try
            {
                var encryptPassword = this.EncodeMD5(password);
                var user            =  _userRepository.Get(x => x.UserName == username && x.Password == encryptPassword);

                if (user == null)
                    throw new Exception("No se encontró usuario con los datos proporcionados");


                response.IsSuccess = true;
                response.Data = new UserApplication
                {
                    EmailAddress   = user.Email,                    
                    Id             = user.Id,                    
                    FullName       = user.FullName,
                    Rol            = user.RolId == 1 ? EnumRoles.Administrator : EnumRoles.Consulting ,
                    UserName       = user.UserName,
                    ChangePassword = user.ChangePassword,
                };
            }
            catch (Exception ex)
            {
                response.IsSuccess     = false;
                response.Messages      = "Problemas al validar el usuario: ";
                response.InternalError = ex.Message;
                
            }

            return response;
        }

        /// <summary>
        /// Agregar un nuevo usuario en la base de datos 
        /// </summary> 
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<User>> AddUser(SaveUserRequest request)
        {
            var response = new Response<User>();

            try
            {
                var user = (new User
                {
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Email      = request.Email.Trim().ToLower(),
                    FullName   = request.FullName,
                    Password   = this.EncodeMD5(request.Password),
                    RolId      = request.RolId,
                    UserName   = request.UserName.Trim().ToLower()
                });

                _userRepository.Add(user);
                _unitOfWork.Commit();

                response = new Response<User>{
                    Data       = user,
                    IsSuccess  = true,
                    Messages   = "Usuario creado correctamente"
                };

            }
            catch (Exception ex)
            {
                response.IsSuccess      = false;
                response.Messages       = "Problemas al validar el usuario";
                response.InternalError  = ex.Message;

            }

            return response;
        }

        /// <summary>
        /// Actualizar los datos de un usuario
        /// </summary>
        /// <param name="request">Datos del usuario</param>
        /// <returns>Objeto User con los datos actualizados</returns>
        public async Task<Response<User>> UpdateUser(UpdateUserRequest request)
        {
            var response = new Response<User>();

            try
            {
                var user = _userRepository.GetById(request.Id);

                if (user == null) throw new Exception("Usuario no encontrado dentro de la based de datos");

                user.FullName   = request.FullName;
                user.RolId      = request.RolId;
                user.UserName   = request.UserName;
                user.UpdateDate = DateTime.Now;
                user.Email      = request.Email;
                
                _userRepository.Update(user);
                _unitOfWork.Commit();

                response = new Response<User>
                {
                    Data      = user,
                    IsSuccess = true,
                    Messages  = "Datos del usuario actualizados correctamente"
                };

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Problemas al actualizar los datos del usuario";
                response.InternalError = ex.Message;

            }

            return response;

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

                    response.Data     = response.IsSuccess = true;
                    response.Messages = "Usuario eliminado correctamente de la base de datos";
                }
                else {

                    response.IsSuccess= response.Data = false;
                    response.Messages                 = "Datos de usuario no encontrados";                    
                }
                
            }
            catch (Exception ex )
            {

                response.IsSuccess     = false;
                response.Messages      = "Error al intentar eliminar el usuario";
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
                response.IsSuccess = true;
                response.Messages  = "Listado de usuario obtenido d emanera exitosa";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages  = "Error al obtener el listado de usuarios";
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
            var response = new Response<User>();

            try
            {
                response.Data = _userRepository.GetById(userId);
                response.IsSuccess = true;
                response.Messages = "Listado de usuario obtenido d emanera exitosa";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Error al obtener el listado de usuarios";
                response.Data = null;

            }

            return response;
        }

        /// <summary>
        ///  Método que permite generar un password temporal
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Response<User>> RecoveryPassword(string email, string bodyHTML) {

            var response = new Response<User>();

            try
            {
                var user =  _userRepository.Get(x => x.Email.Trim() == email.Trim());

                if (user == null) {
                    response.Messages  = $"El usuario con correo eléctronico { email } no ha sido encontrado, favor de corroborar la información";
                    response.IsSuccess = false;

                    return response;
                }

                var password        = this.RandomString(18);
                user.Password       = this.EncodeMD5(password);
                user.UpdateDate     = DateTime.Now;
                user.Email          = user.Email.Trim();
                user.UserName       = user.UserName.Trim();
                user.ChangePassword = true;

                _userRepository.Update(user);
                _unitOfWork.SaveChanges();

                bodyHTML  = bodyHTML.Replace("[token]", password);
                MailService.SendMessage(email, bodyHTML, "Password");

                response.IsSuccess = true;
                response.Messages  = "Se han generado una contraseña temporal la cuál se mando al correo proporcionado ";

            }
            catch (Exception ex)
            {
                response.Data      = null;
                response.IsSuccess = false;
                response.Messages  = ex.Message;
            }

            return response;

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
                var encrypLastPassword = this.EncodeMD5(request.LastPassword); 
                var user = await _userRepository.FindAsync(x => x.Id == userId && x.Password == encrypLastPassword);

                if (user == null)
                {
                    response.Messages = $"El usuario no ha sido encontrado, favor de corroborar la información";
                    response.IsSuccess = false;

                    return response;
                }
                                
                user.Password       = this.EncodeMD5(request.NewPassword);
                user.UpdateDate     = DateTime.Now;
                user.ChangePassword = false;

                _userRepository.Update(user);
                _unitOfWork.SaveChanges();

                response.IsSuccess = true;
                response.Messages  = "Se actualizó correctamente la contraseña del usuario";

            }
            catch (Exception ex)
            {
                response.Data       = false;
                response.IsSuccess  = false;
                response.Messages   = ex.Message;
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
        private string EncodeMD5(string passowd)
        {                        
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
                        
            md5           = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(passowd);
            encodedBytes  = md5.ComputeHash(originalBytes);
                    
            return BitConverter.ToString(encodedBytes);
        }

        /// <summary>
        /// Create password ramdom
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion

    }
}
