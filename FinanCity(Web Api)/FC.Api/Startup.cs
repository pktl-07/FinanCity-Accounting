using System;
using System.Web.Http;
using System.Web.Cors;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Ajax.Utilities;
using Owin;
using Ninject;
using FC.Core;
using FC.Api.Infrastructure;
using FC.Core.Infrastructure.ConfigurationManager;

[assembly: OwinStartup(typeof(FC.Api.Startup))]
namespace FC.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            var config = DI.Current.Get<FinanCityConfig>();
            var cors = new EnableCorsAttribute(config.GeneralConfiguration.DomainUrl,"*", "*");

            SignalRConfig.Register(app, cors);

            var authOption = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1440),
                Provider = DI.Current.Get<FCAuthorizationServerProvider>()
            };

            //Token Generations
            app.UseOAuthAuthorizationServer(authOption);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            var httpConfig = new HttpConfiguration();
            WebApiConfig.Register(httpConfig);
        }
    }

    public class SignalRConfig
    {
        public static void Register(IAppBuilder app, EnableCorsAttribute cors)
        {
            app.Map("/signalr", map =>
            {
                var corsOptions = new CorsOptions
                {
                    PolicyProvider = new CorsPolicyProvider
                    {
                        PolicyResolver = context =>
                        {
                            var policy = new CorsPolicy
                            {
                                AllowAnyHeader = true,
                                AllowAnyMethod = true,
                                SupportsCredentials = true
                            };
                            cors.Origins.ForEach(o => policy.Origins.Add(o));
                            return Task.FromResult(policy);
                        }
                    }
                };
                map.UseCors(corsOptions).RunSignalR();
            });
        }
    }
}
