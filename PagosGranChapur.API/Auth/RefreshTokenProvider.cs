using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PagosGranChapur.API.Auth
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var addAsyncTask = new TaskCompletionSource<AuthenticationTokenCreateContext>();
            var guid = Guid.NewGuid().ToString();

            // copy all properties and set the desired lifetime of refresh token  
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)//DateTime.UtcNow.AddYears(1)  
            };
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle  
            context.SetToken(guid);
            addAsyncTask.SetResult(context);

            await addAsyncTask.Task;
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var receiveAsyncTask = new TaskCompletionSource<AuthenticationTokenReceiveContext>();
            AuthenticationTicket ticket;

            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
            receiveAsyncTask.SetResult(context);

            await receiveAsyncTask.Task;
        }
    }

}