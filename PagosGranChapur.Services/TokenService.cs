using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Entities.WebServerRequest;
using PagosGranChapur.Entities.WebServerResponses;
using PagosGranChapur.Services.Helpers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PagosGranChapur.Services
{
    public interface ITokenService {

        Task<Response<TokenWSResponse>> CreateToken(TokenRequest request, string baseURL, string apiUrl,string bodyHTML);
    }

    public class TokenService: ITokenService
    {
        public TokenService() { }

        #region INTERFACE METHODS

        /// <summary>
        /// GENERA UN NUEVO TOKEN Y ES ENVIADO AL USUARIO POR MEDIO DE SMS
        /// </summary>
        /// <param name="request"> Parámetros enviados desde el cliente </param>
        /// <param name="baseURL"> URL base donde se encuentra el servicio API de CHAPUR </param>
        /// <param name="apiUrl"> Método del API </param>
        /// <returns>Response<string></returns>
        public async Task<Response<TokenWSResponse>> CreateToken(TokenRequest request,string baseURL, string apiUrl, string bodyHTML)
        {
            var response = new Response<TokenWSResponse>();

            try
            {
                var tokenRequest = new TokenWSRequest
                {
                    Tarjeta = request.NoCreditCard,
                    Accion  = request.Action,
                    Tienda  = request.StoreId
                };
                                
                var tokenResponse  = await RequestService.Post<TokenWSRequest, TokenWSResponse >(apiUrl, baseURL, tokenRequest);
                                
                if (tokenResponse != null && tokenResponse.Estatus == 0)
                {
                    response.IsSuccess = true;
                    response.Messages  = "Token generado correctamente";
                    tokenResponse.CorreoElectronico = request.Email;
                    response.Data      = tokenResponse;

                } else {

                    response.Messages  = "Error al generar el token";
                    response.IsSuccess = false;

                    if (tokenResponse == null) {
                        throw new Exception("Error al conectarse con el servicio en CHAPUR");
                    }

                    if (tokenResponse.Token != null && !tokenResponse.Equals(""))
                    {
                        if (request.Email != "")
                        {
                            bodyHTML = bodyHTML.Replace("[token]", tokenResponse.Token);
                            MailService.SendMessage(request.Email, bodyHTML,"Token");

                            tokenResponse.Telefono          = null;
                            tokenResponse.CorreoElectronico = request.Email;
                            response.Messages               = "Se generó corretamente y fue enviado al correo";
                            response.IsSuccess              = true;
                        }
                    }      

                    response.Data      = tokenResponse;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess     = false;
                response.Messages      = "Problemas al generar el Token";
                response.InternalError = ex.Message;
            }

            return response;
        }

        #endregion
    }
}
