using System;

namespace PagosGranChapur.Data
{
    public interface IDbFactory : IDisposable
    {
        PagosGranChapurContext Init();
    }
}
