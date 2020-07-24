using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using PagosGranChapur.Entities;
using PagosGranChapur.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PagosGranChapur.API.Auth
{
    public class ApplicationOAuthServerProvider : OAuthAuthorizationServerProvider
    {        
        private UserApplication _user;
        private IUserService _srvUser;

        public ApplicationOAuthServerProvider() {}

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            var autofacLifetimeScope = OwinContextExtensions.GetAutofacLifetimeScope(context.OwinContext);
            _srvUser                 = autofacLifetimeScope.Resolve<IUserService>();

            await Task.FromResult(context.Validated());
        }
       
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                //Validacion
                if (context.UserName == "" || context.Password == "")
                {
                    context.SetError("Usuario y/0 contraseña son requeridos");
                    context.SetError("validation_user", "Usuario y/o contraseña son obligatorios");
                    return;
                }
                                
                var userResponse = await _srvUser.ValidateUser(context.UserName, context.Password);
                if (userResponse != null)
                {
                    if (userResponse.IsSuccess)
                    {
                        this._user = userResponse.Data;
                    }
                    else
                    {
                        context.SetError("validation_user", $"Usuario y/o contraseña no validos: { userResponse.InternalError }");
                        return;
                    }
                }
                else
                {
                    context.SetError("validation_user", "Error de conexión");
                    return;
                }

                ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("user_name", this._user.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, this._user.Rol.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, this._user.Id.ToString()));

                AuthenticationProperties properties = CreateProperties(this._user);
                AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);

                context.Validated(ticket);
            }
            catch (Exception ex)
            {
                context.SetError("validation_user", ex.Message);
                throw;
            }
          
        }

        public static AuthenticationProperties CreateProperties(UserApplication user)
        {
            IDictionary<string, string> data = new Dictionary<string, string>{
                { "userName", user.UserName },
                { "email", user.EmailAddress },
                { "fullName", user.FullName },
                { "rol", user.Rol.ToString() },
                { "changePassword", user.ChangePassword.ToString() },
            };

            return new AuthenticationProperties(data);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
 
            // Change authentication ticket for refresh token requests  
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override Task MatchEndpoint(OAuthMatchEndpointContext context) {          
            if (context.Request.Method == "OPTIONS")
            {
                context.RequestCompleted();
                return Task.FromResult(0);
            }
            
            return base.MatchEndpoint(context);            
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}