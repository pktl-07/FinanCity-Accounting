namespace FC.Api
{
    using FC.Core;
    using Ninject;
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;
 
    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<TaxValidator>().To<TaxValidator>();
            kernal.Bind<CashbookSetupValidator>().To<CashbookSetupValidator>();
            kernal.Bind<DepartmentSetupValidator>().To<DepartmentSetupValidator>();
            kernal.Bind<DepartmentSetupDeleteValidator>().To<DepartmentSetupDeleteValidator>();
            kernal.Bind<AccountingPeriodValidator>().To<AccountingPeriodValidator>();
            kernal.Bind<AccountingPeriodUpdateValidator>().To<AccountingPeriodUpdateValidator>();
            kernal.Bind<JournalLedgerValidator>().To<JournalLedgerValidator>();
            kernal.Bind<JournalLedgerReverseValidator>().To<JournalLedgerReverseValidator>();
            kernal.Bind<CashbookTransferValidator>().To<CashbookTransferValidator>();
            kernal.Bind<CashbookTransactionValidator>().To<CashbookTransactionValidator>();
        }
    }
}
