using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace PagosGranChapur.API.Controllers
{
    public class BaseController: ApiController
    {
        //OBTENER EL IDENTIFICADOR DEL USUARIO ACTUAL
        protected int UserId
        {
            get
            {
                var claimId     = User.Identity.Name;
                var claimResult = int.TryParse(claimId, out int userId);

                return claimResult ? userId : 0;
            }
        }      
    }

    
}