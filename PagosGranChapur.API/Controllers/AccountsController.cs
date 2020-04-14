using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Entities.Responses;
using PagosGranChapur.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace PagosGranChapur.API.Controllers
{
    [RoutePrefix("api/v0/Accounts")]
    
    public class AccountsController : BaseController
    {
        private readonly ITestService _testSetvice;
        private readonly ICryptoMessage _cryptoMessage;

        public AccountsController(ITestService testSetvice,
                                  ICryptoMessage cryptoMessage) {

            _testSetvice   = testSetvice;
            _cryptoMessage = cryptoMessage;
        }
        
        [Authorize]
        [HttpPost]
        [Route("test")]
        public async Task<IHttpActionResult> Token()
        {
            var response       = new Response<List<string>>();
            var srvValue       = await _testSetvice.GetName();
            var user = this.UserId;

            response.Data      = new List<string> {
                _cryptoMessage.EncryptStringAES("1"),
                _cryptoMessage.EncryptStringAES("2"),
                _cryptoMessage.EncryptStringAES("3"),
                _cryptoMessage.EncryptStringAES("4")
            };
            
            response.IsSuccess = true;
            response.Messages  = _cryptoMessage.EncryptStringAES($"{UserId.ToString()} {srvValue}");
            
            return Ok(response);
        }
    }
}
