using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using PagosGranChapur.API.Auth;
using PagosGranChapur.Data;
using PagosGranChapur.Data.Infrastructure;
using PagosGranChapur.Entities.Helpers;
using PagosGranChapur.Repositories;
using PagosGranChapur.Services;
using System;
using System.Reflection;
using System.Web.Http;

[assembly: OwinStartup(typeof(PagosGranChapur.API.Startup))]
namespace PagosGranChapur.API
{
    public partial class Startup
    {
        private static IContainer ContainerIoC;

        public void Configuration(IAppBuilder app)
        {   
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            
            #region AUTOFACT CONFIG

            var builder = new ContainerBuilder();

            // Register Web API controller in executing assembly.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();
            builder.RegisterType<CryptoMessage>().As<ICryptoMessage>().InstancePerRequest();
          
            #region REPOSITORIES            
            builder.RegisterType<TransactionRepository>().As<ITransactionRepository>().InstancePerRequest();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerRequest();
            builder.RegisterType<LogTransactionRepository>().As<ILogTransactionRepository>().InstancePerRequest();
            builder.RegisterType<StoreRepository>().As<IStoreRepository>().InstancePerRequest();
            builder.RegisterType<PlatformRepository>().As<IPlatformRepository>().InstancePerRequest();
            #endregion

            #region SERVICES            
            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<TokenService>().As<ITokenService>().InstancePerRequest();
            builder.RegisterType<TransactionService>().As<ITransactionService>().InstancePerRequest();
            builder.RegisterType<StoreService>().As<IStoreService>().InstancePerRequest();
            builder.RegisterType<PlatformService>().As<IPlatformService>().InstancePerRequest();
            #endregion

            // Create and assign a dependency resolver for Web API to use.
            ContainerIoC = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(ContainerIoC);
            app.UseAutofacMiddleware(ContainerIoC);            

            #endregion
            
            ConfigureAuth(app);                       
            app.UseWebApi(config);
            
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath         = new PathString("/token"),
                Provider                  = new ApplicationOAuthServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
                AllowInsecureHttp         = true,
                RefreshTokenProvider      = new RefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            
            
        }
        
    }
}
