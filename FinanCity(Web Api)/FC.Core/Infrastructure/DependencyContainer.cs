namespace FC.Core.Infrastructure
{
    using Implementation;
    using Ninject;
   
    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind(typeof(IDataAnalyzerBuilder<>), typeof(DataAnalyzerBuilder<>));
        }
    }
}
