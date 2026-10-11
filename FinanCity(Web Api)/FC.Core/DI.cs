namespace FC.Core
{
    using Ninject;
    using Ninject.Modules;

    public abstract class DI
    {
        public DI() { }

        public abstract void RegisterService(IKernel kernal);

        public static IKernel Current
        {
            get;
            internal set;
        }
    }
}
