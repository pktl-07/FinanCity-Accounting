namespace FC.Api
{
    using FC.Core;
    using FC.Api.SalesAreas.Areas.Validators;
    using Ninject;

    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<CommissionSchemeValidator>().To<CommissionSchemeValidator>();
            kernal.Bind<ClientSetupValidator>().To<ClientSetupValidator>();
            kernal.Bind<VendorSetupValidator>().To<VendorSetupValidator>();
            kernal.Bind<QualificationValidator>().To<QualificationValidator>();
            kernal.Bind<IndustryExamValidator>().To<IndustryExamValidator>();
            kernal.Bind<ExternalSalespersonValidator>().To<ExternalSalespersonValidator>();
            kernal.Bind<MyCompanyValidator>().To<MyCompanyValidator>();
            kernal.Bind<SalespersonManagementValidator>().To<SalespersonManagementValidator>();
            kernal.Bind<CurrencyRateValidator>().To<CurrencyRateValidator>();
            kernal.Bind<PropertyTransactionValidator>().To<PropertyTransactionValidator>();
            kernal.Bind<SalesInvoiceValidator>().To<SalesInvoiceValidator>();
            kernal.Bind<CommissionReceivableValidator>().To<CommissionReceivableValidator>();
            kernal.Bind<CommissionReceivedAllocationValidator>().To<CommissionReceivedAllocationValidator>();
            kernal.Bind<CommissionPayableValidator>().To<CommissionPayableValidator>();
            kernal.Bind<CommissionSchemeDeleteValidator>().To<CommissionSchemeDeleteValidator>();
            kernal.Bind<PromotionItemValidator>().To<PromotionItemValidator>();
            kernal.Bind<MarketingPromotionAllocationValidator>().To<MarketingPromotionAllocationValidator>();
            kernal.Bind<CareerManagementValidator>().To<CareerManagementValidator>();
            kernal.Bind<SalesOrderTypeValidator>().To<SalesOrderTypeValidator>();
            kernal.Bind<ReferralSalesOrderValidator>().To<ReferralSalesOrderValidator>();
            kernal.Bind<BankBestRateValidator>().To<BankBestRateValidator>();
            kernal.Bind<LeadValidation>().To<LeadValidation>();
        }
    }
}
