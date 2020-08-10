using PagosGranChapur.Entities;
using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            var validaSessionTask = new Task<IHttpActionResult>(() =>
            {
                return Ok();
            });

            return await validaSessionTask;
        }

        /// <summary>
        /// API para guardar los datos de un nuevo usuario
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Save(SaveUserRequest request)
        {
            Response<User> response;

            try
            {
                if (UserId == 0) return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                response = await _userService.AddUser(request);

            }
            catch (Exception ex)
            {
                response = ResponseConverter.ToExceptionResponse<User>(ex);
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
            Response<List<User>> response;

            try
            {
                if (UserId == 0) return BadRequest();

                response = await _userService.GetAll();
            }
            catch (Exception ex)
            {
                response = ResponseConverter.ToExceptionResponse<List<User>>(ex);
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
            Response<User> response;

            try
            {
                if (UserId == 0) return BadRequest();

                response = await _userService.GetById(userId);
            }
            catch (Exception ex)
            {
                response = ResponseConverter.ToExceptionResponse<User>(ex);
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
            Response<User> response;

            try
            {
                if (UserId == 0) return BadRequest();

                response = await _userService.GetById(this.UserId);
            }
            catch (Exception ex)
            {
                response = ResponseConverter.ToExceptionResponse<User>(ex);
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
            Response<User> response;

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
                response = ResponseConverter.ToExceptionResponse<User>(ex);
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

            Response<User> response;

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                string bodyHTML = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["API.Email.Template"]));
                response = await _userService.RecoveryPassword(request.Email, bodyHTML);
            }
            catch (Exception ex)
            {
                response = ResponseConverter.ToExceptionResponse<User>(ex);
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
            Response<bool> response;

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
                response = ResponseConverter.ToExceptionResponse<bool>(ex);
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
            Response<bool> response;

            try
            {
                if (UserId == 0) return BadRequest();

                if (userId == 0)
                    throw new PagosChapurException("El identificador del usuario es obligatorio para eliminar los datos");

                response = await _userService.DeleteUser(userId);

            }
            catch (Exception ex)
            {
                response = ResponseConverter.ToExceptionResponse<bool>(ex);
            }

            return Ok(response);
        }

    }
}
