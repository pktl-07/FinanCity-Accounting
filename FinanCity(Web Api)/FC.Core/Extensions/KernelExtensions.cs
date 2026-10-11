namespace FC.Core.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Ninject;
    using FC.Core.Infrastructure;
    using FC.Core.Infrastructure.Implementation;
    
    public static class KernelExtensions
    {
        public static void AddEartStone(this IKernel services, params string[] assemblynames)
        {
            Guard.IsNotNull(assemblynames, "assemblyname");

            //especially to use for resolving as it needs to help resolved in ess framework to do.
            DI.Current = services;

            //find out all assembly which derive from DI abstract base and forcing to read the Register method at run time.
            assemblynames.Each(asm =>
            {
                Assembly.Load(asm).GetTypes().Where(x => x.IsClass && x.BaseType == typeof(DI)).Each(type =>
                {
                    type.GetMethod("RegisterService")
                    .Invoke(Activator.CreateInstance(type.GetTypeInfo()), new object[] { services });
                });
            });
        }

        static EssAppServices _appsetting = new EssAppServices();
        static EssDataServices _dataservices = new EssDataServices();

        public static void UseEarthStone(this IKernel kernal, 
            Action<EssAppServices> appsetting, 
            Action<EssDataServices> dataservice)
        {
            appsetting(_appsetting);
            dataservice(_dataservices);

            kernal.Bind<IEssAppRepository, EssAppServices>().ToConstant(_appsetting);
            kernal.Bind<IEssDataRepository, EssDataServices>().ToConstant(_dataservices);
        }
    }
}
