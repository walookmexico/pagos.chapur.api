using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Entities.WebServerResponses;
using PagosGranChapur.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PagosGranChapur.API.Controllers
{
    [RoutePrefix("api/v0/User")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) {
            _userService = userService;
        }

        /// <summary>
        /// Validar la sesion del usuario 
        /// </summary>
        /// <returns></returns>
        [Route("ValidateSession")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ValidateSession()
        {
            return Ok();
        }

        /// <summary>
        /// API para guardar los datos de un nuevo usuario
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]        
        public async Task<IHttpActionResult> Save(SaveUserRequest request)
        {
            var response = new Response<User>();

            try
            {
                if (UserId == 0) return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                response = await _userService.AddUser(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess     = false;
                response.InternalError = ex.Message;
            }

            return Ok(response);
        }
        
        /// <summary>
        /// API para obtener los datos de todos los usuarios registrados
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]        
        public async Task<IHttpActionResult> GetUser()
        {
            var response = new Response<List<User>>();

            try
            {
                if (UserId == 0) return BadRequest();

                response = await _userService.GetAll();                
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = ex.Message;
            }

            return Ok(response);

        }

        /// <summary>
        /// Obtiene los datos de un usuario en especifico por su identificador
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]        
        public async Task<IHttpActionResult> GetUser(int userId)
        {
            var response = new Response<User>();

            try
            {
                if (UserId == 0) return BadRequest();

                response = await _userService.GetById(userId);               
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = ex.Message;
            }

            return Ok(response);
           
        }

        /// <summary>
        /// API para obtener los datos del suuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        [Authorize(Roles = "Administrator, Consulting")]
        [Route("GetPerfil")]
        public async Task<IHttpActionResult> GetPerfil()
        {
            var response = new Response<User>();

            try
            {
                if (UserId == 0) return BadRequest();

                response = await _userService.GetById(this.UserId);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = ex.Message;
            }

            return Ok(response);

        }
        
        /// <summary>
        /// API que permite actualizar los datos de un usuario
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Administrator")]        
        public async Task<IHttpActionResult> Update(UpdateUserRequest request)
        {
            var response = new Response<User>();

            try
            {
                if (UserId == 0) return BadRequest();

                if (request == null) return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                               
                response = await _userService.UpdateUser(request);

            }
            catch (Exception ex)
            {
                response.IsSuccess     = false;
                response.InternalError = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// API para generar una contraseña temporal y es enviada al correo electrónico.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("RecoveryPassword")]
        public async Task<IHttpActionResult> RecoveryPassword(TempPassowrdRequest request)
        {

            var response = new Response<User>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                string bodyHTML = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["API.Email.Template"]));
                response = await _userService.RecoveryPassword(request.Email, bodyHTML);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.InternalError = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// API para actualizar la contraseña y password del usuario.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, Consulting")]
        [Route("UpdatePassword")]
        public async Task<IHttpActionResult> UpdatePassword(UpdatePasswordRequest request)
        {
            var response = new Response<bool>();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                if (UserId == 0)
                    return BadRequest();
                
                response = await _userService.UpdatePasword(UserId, request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.InternalError = ex.Message;
            }

            return Ok(response);
        }

        /// <summary>
        /// API que permite eliminar un usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Administrator")]        
        public async Task<IHttpActionResult> Delete(int userId)
        {
            var response = new Response<bool>();

            try
            {
                if (UserId == 0) return BadRequest();

                if (userId == 0)
                    throw new Exception("El identificador del usuario es obligatorio para eliminar los datos");

                response = await _userService.DeleteUser(userId);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.InternalError = ex.Message;
            }

            return Ok(response);
        }

 
    }
}
