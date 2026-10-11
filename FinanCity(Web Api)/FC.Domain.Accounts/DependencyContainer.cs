namespace FC.Domain.Accounts
{
    using FC.Core;
    using Ninject;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Business.ChartAccounts;
    using FC.Domain.Accounts.Infrastructure;
    using FC.Domain.Accounts.Entity;

    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<AccountSetup>().To<AccountSetup>();
            kernal.Bind<AccountCategory>().To<AccountCategory>();
            kernal.Bind<Tax>().To<Tax>();
            kernal.Bind<Department>().To<Department>();
            kernal.Bind<CashbookSetup>().To<CashbookSetup>();
            kernal.Bind<AccountingPeriod>().To<AccountingPeriod>();
            kernal.Bind<BatchModule>().To<BatchModule>();
            kernal.Bind<BatchControl>().To<BatchControl>();
            kernal.Bind<BTable>().To<BTable>();
            kernal.Bind<HierarchyChartBuilder>().To<HierarchyChartBuilder>();
            kernal.Bind<YearClosing>().To<YearClosing>();
            kernal.Bind<CashBookTransfer>().To<CashBookTransfer>();
            kernal.Bind<CashBookTransaction>().To<CashBookTransaction>();
            kernal.Bind<Job>().To<Job>();

            kernal.Bind<IBTableResult<CashBookTransactionEntity>>()
                .To<CashbookTransactionAccountResult>();

            kernal.Bind<IBTableResult<CashBookTransferEntity>>()
               .To<CashbookTransferAccountResult>();
        
        }
    }
}
