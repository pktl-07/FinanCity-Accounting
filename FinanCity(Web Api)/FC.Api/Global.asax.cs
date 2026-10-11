namespace FC.Api
{
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using FC.Core;
    using FC.Api.Infrastructure;
   
    public class WebApiApplication : System.Web.HttpApplication
    {
        private string connection;

        public WebApiApplication()
        {
            connection = ConfigurationManager.ConnectionStrings[CommonConstant.SalesConnection].ConnectionString;
        }

        protected async void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SqlDependency.Start(connection);
            
            await FinanCityGlobleSetting.AsyncRegisterAllConfiguration();
        }

        protected void Application_End()
        {
            SqlDependency.Stop(connection); // end notify sql
        }
    }
}
