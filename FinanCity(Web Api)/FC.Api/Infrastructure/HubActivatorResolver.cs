namespace FC.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject;
    using Microsoft.AspNet.SignalR.Hubs;
    using Microsoft.AspNet.SignalR;
  
    public class NinjectSignalRDependencyResolver : DefaultDependencyResolver
    {
        private IKernel _kernal;
        public NinjectSignalRDependencyResolver(IKernel kernal)
        {
            _kernal = kernal;
        }

        public override object GetService(Type serviceType)
        {
            return _kernal.TryGet(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernal.GetAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }

    public class HubActivatorResolver : IHubActivator
    {
        private readonly IKernel _kernal;

        public HubActivatorResolver(IKernel kernal)
        {
            _kernal = kernal;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return (IHub)_kernal.GetService(descriptor.HubType);
        }
    }
}