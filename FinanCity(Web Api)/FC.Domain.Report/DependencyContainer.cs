namespace FC.Domain.Report
{
    using FC.Core;
    using Ninject;
    using FC.Domain.Report.Business;
  
    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<AccountingReport>().To<AccountingReport>();
            kernal.Bind<SalesReport>().To<SalesReport>();
            kernal.Bind<DateRangeFor>().To<DateRangeFor>();
            kernal.Bind<SalespersonFor>().To<SalespersonFor>();
            kernal.Bind<ReceivableStatusFor>().To<ReceivableStatusFor>();
            kernal.Bind<IRegisterFormulaFieldFactory>().To<RegisterFormulaFieldFactory>();
            kernal.Bind<TNC_PropertyFormulaFieldFor>().To<TNC_PropertyFormulaFieldFor>();
            kernal.Bind<TNC_TransactionFormulaFieldFor>().To<TNC_TransactionFormulaFieldFor>();
            kernal.Bind<PayableStatusFor>().To<PayableStatusFor>();
            kernal.Bind<TopFor>().To<TopFor>();
            kernal.Bind<SalesBonusReport>().To<SalesBonusReport>();
            kernal.Bind<ClientFor>().To<ClientFor>();
            kernal.Bind<AgedReceivables>().To<AgedReceivables>();
            kernal.Bind<FinancialReport>().To<FinancialReport>();
            kernal.Bind<SalespersonTypeFor>().To<SalespersonTypeFor>();
            kernal.Bind<CobrokerAccuralChangeDateFor>().To<CobrokerAccuralChangeDateFor>();
            kernal.Bind<TaxFor>().To<TaxFor>();
            kernal.Bind<AccountCodeFor>().To<AccountCodeFor>();
            kernal.Bind<YCStartFor>().To<YCStartFor>();
            kernal.Bind<YCEndFor>().To<YCEndFor>();
            kernal.Bind<CashbookReport>().To<CashbookReport>();
            kernal.Bind<TransTypeFor>().To<TransTypeFor>();
            kernal.Bind<JobReport>().To<JobReport>();
            kernal.Bind<JobFor>().To<JobFor>();
            kernal.Bind<VendorFor>().To<VendorFor>();
            kernal.Bind<RecordStatusFor>().To<RecordStatusFor>();
            kernal.Bind<RankTypeFor>().To<RankTypeFor>();
            kernal.Bind<PaidDateFor>().To<PaidDateFor>();
            kernal.Bind<GLRefFor>().To<GLRefFor>();
            kernal.Bind<SecondRefFor>().To<SecondRefFor>();
            kernal.Bind<T1For>().To<T1For>();
            kernal.Bind<FranchiseeFor>().To<FranchiseeFor>();
        }
    }
}
