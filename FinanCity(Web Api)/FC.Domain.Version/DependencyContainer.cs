namespace FC.Domain.Version
{
    using FC.Core;
    using Ninject;
    using FC.Domain.Version.Business;
 
    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<Versions>().To<Versions>();
        }
    }
}
