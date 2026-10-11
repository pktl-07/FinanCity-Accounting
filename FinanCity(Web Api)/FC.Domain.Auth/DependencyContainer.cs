namespace FC.Domain.Auth
{
    using FC.Core;
    using FC.Domain.Auth.Business;
    using Ninject;
  
    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<UserAccount>().To<UserAccount>();
        }
    }
}
