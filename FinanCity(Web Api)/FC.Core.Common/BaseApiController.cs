namespace FC.Core
{
    using System.Web.Http;
    using Ninject;
    using FC.Core.Infrastructure.Interfaces;
  
    public class BaseApiController : ApiController
    {
        #region Fields

        private readonly IFinanCityIdentity _finanCityIdentity = DI.Current.Get<FinanCityIdentity>();
        public IFinanCityIdentity FinanCityIdentity
        {
            get => _finanCityIdentity;
        }

        #endregion

        #region Ctor

        public BaseApiController()
        {
            DI.Current.Get<IEssDataRepository>().LoginName = _finanCityIdentity.Email;
            DI.Current.Get<IEssAppRepository>().SPSK = _finanCityIdentity.SPSK;
        }

        #endregion
    }
}
