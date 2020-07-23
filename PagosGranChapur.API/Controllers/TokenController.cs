using PagosGranChapur.Entities.Request;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Entities.WebServerResponses;
using PagosGranChapur.Services;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace PagosGranChapur.API.Controllers
{
    [RoutePrefix("api/v0/Token")]
    public class TokenController: BaseController
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService) {
            this._tokenService = tokenService;
        }

        /// <summary>
        /// API que permite generar un nuevo TOKEN de confirmación al usuario solicitante
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]        
        [Route("Create")]
        public async Task<IHttpActionResult> Create(TokenRequest request)
        {
            var response = new Response<TokenWSResponse>();
            
            if(request == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string bodyHTML = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["API.Email.Template"]));

            response = await this._tokenService.CreateToken(request,ConfigurationManager.AppSettings["Chapur.API.BaseURL"],
                                                            ConfigurationManager.AppSettings["Chapur.API.Token"], bodyHTML);

            if (ConfigurationManager.AppSettings["API.Env"].ToString() == "prod")
            {
                if (response.Data.Token != null)
                {
                    response.Data.Token = this.ReplaceCharacter(response.Data.Token);
                    response.Data.Telefono = "**********".Substring(0, response.Data.Telefono.Length - 4) + response.Data.Telefono.Substring(response.Data.Telefono.Length - 4, 4);
                }
            }

            return Ok(response);
        }


        private string ReplaceCharacter(string word, string character= "*") {

            var newWord = "";
            word.ToCharArray().ToList().ForEach(x => newWord += character);
            return newWord;


        }

    }
}