namespace FC.Api
{
    using Ninject;
    using FC.Api.Infrastructure.FluentValidators;
    using FC.Api.Infrastructure;
    using FC.Core;

    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<FCAuthorizationServerProvider>().To<FCAuthorizationServerProvider>();
            kernal.Bind<UserAccountValidator>().To<UserAccountValidator>();
            kernal.Bind<DeleteUserAccountValidator>().To<DeleteUserAccountValidator>();
            kernal.Bind<ChangePasswordValidator>().To<ChangePasswordValidator>();
            kernal.Bind<ResetPasswordValidator>().To<ResetPasswordValidator>();
        }
    }
}
