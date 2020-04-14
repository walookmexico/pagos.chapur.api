using System;
using System.Collections.Generic;
using System.Text;

namespace PagosGranChapur.Data
{
    public interface IDbFactory : IDisposable
    {
        PagosGranChapurContext Init();
    }
}
