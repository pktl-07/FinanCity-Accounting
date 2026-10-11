namespace FC.Domain.Sales
{
    using FC.Core;
    using Ninject;
    using FC.Domain.Sales.Business;
    using FC.Domain.Accounts;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Sales.Infrastructure;
  
    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<Company>().To<Company>();
            kernal.Bind<CommissionStructureDetail>().To<CommissionStructureDetail>();
            kernal.Bind<CommissionStructure>().To<CommissionStructure>();
            kernal.Bind<Person>().To<Person>();
            kernal.Bind<Contact>().To<Contact>();
            kernal.Bind<Country>().To<Country>();
            kernal.Bind<IDIndexing>().To<IDIndexing>();
            kernal.Bind<ReferenceIndexTree>().To<ReferenceIndexTree>();
            kernal.Bind<Qualification>().To<Qualification>();
            kernal.Bind<IndustryExam>().To<IndustryExam>();
            kernal.Bind<Employment>().To<Employment>();
            kernal.Bind<PersonIndustry>().To<PersonIndustry>();
            kernal.Bind<PersonEducation>().To<PersonEducation>();
            kernal.Bind<CommissionAllocation>().To<CommissionAllocation>();
            kernal.Bind<OverridingProfile>().To<OverridingProfile>();

            kernal.Bind<SelespersonWhoHasResetScheme>().To<SelespersonWhoHasResetScheme>();
            kernal.Bind<SalespersonWhoDoNotHaveResetScheme>().To<SalespersonWhoDoNotHaveResetScheme>();

            kernal.Bind<EmploymentHistory>().To<EmploymentHistory>();
            kernal.Bind<PropertyTransaction>().To<PropertyTransaction>();
            kernal.Bind<PropertyTransactionDetail>().To<PropertyTransactionDetail>();
            kernal.Bind<PTOClient>().To<PTOClient>();
            kernal.Bind<PostalCode>().To<PostalCode>();
            kernal.Bind<Currency>().To<Currency>();
            kernal.Bind<CurrencyRate>().To<CurrencyRate>();
            kernal.Bind<OverridingScheme>().To<OverridingScheme>();
            kernal.Bind<SalesInvoice>().To<SalesInvoice>();
            kernal.Bind<CurrencyTransaction>().To<CurrencyTransaction>();
            kernal.Bind<PaymentTerm>().To<PaymentTerm>();
            kernal.Bind<CommissionReceived>().To<CommissionReceived>();

            kernal.Bind<TreeBuilder>().To<TreeBuilder>();
            kernal.Bind<TreeBuilderBase>().To<TreeBuilderBase>();
            kernal.Bind<UpTree>().To<UpTree>();
            kernal.Bind<DownTree>().To<DownTree>();
            kernal.Bind<FullTree>().To<FullTree>();
            kernal.Bind<OverridingSchemeGenerator>().To<OverridingSchemeGenerator>();
            kernal.Bind<OverridingSchemeFactory>().To<OverridingSchemeFactory>();
            kernal.Bind<PayableScheme>().To<PayableScheme>();
            kernal.Bind<BonusPayable>().To<BonusPayable>();
            kernal.Bind<Logger>().To<Logger>();
            kernal.Bind<CommissionPayment>().To<CommissionPayment>();
            kernal.Bind<TNC>().To<TNC>();
            kernal.Bind<TNC_InterestRateType>().To<TNC_InterestRateType>();
            kernal.Bind<TNC_PropertyPurpose>().To<TNC_PropertyPurpose>();
            kernal.Bind<TNC_PropertyType>().To<TNC_PropertyType>();
            kernal.Bind<TNC_TransactionType>().To<TNC_TransactionType>();
            kernal.Bind<TNC_RateType>().To<TNC_RateType>();
            kernal.Bind<TNC_Duration>().To<TNC_Duration>();
            kernal.Bind<TNC_TimeFrame>().To<TNC_TimeFrame>();
            kernal.Bind<EmploymentLogger>().To<EmploymentLogger>();
            kernal.Bind<SalespersonLogger>().To<SalespersonLogger>();
            kernal.Bind<ClientShare>().To<ClientShare>();
            kernal.Bind<MarketingItem>().To<MarketingItem>();
            kernal.Bind<PayNow>().To<PayNow>();
            kernal.Bind<MarketingExpense>().To<MarketingExpense>();
            kernal.Bind<DataList>().To<DataList>();
            kernal.Bind<CommissionStructureBase>().To<CommissionStructureBase>();

            kernal.Bind<TireContext>().To<TireContext>();
            kernal.Bind<FixedContext>().To<FixedContext>();
            kernal.Bind<ExternalContext>().To<ExternalContext>();
            kernal.Bind<CommissionHelperFactory>().To<CommissionHelperFactory>();
            kernal.Bind<BonusPaymentDetail>().To<BonusPaymentDetail>();
            kernal.Bind<BonusPayment>().To<BonusPayment>();
            kernal.Bind<IBTableResult<CommissionPaymentEntity>>().To<CommissionPayableAccountResult>();
            kernal.Bind<IBTableResult<CommissionReceivedEntity>>().To<CommissionReceivableAccountResult>();
            kernal.Bind<IBTableResult<AllocationEntity>>().To<CommissionAllocationAccountResult>();
            kernal.Bind<IBTableResult<SalesInvoiceEntity>>().To<SalesInvoiceAccountResult>();
            kernal.Bind<CareerManagement>().To<CareerManagement>();
            kernal.Bind<SalesOrderType>().To<SalesOrderType>();
            kernal.Bind<BankBestRate>().To<BankBestRate>();

            //kernal.Bind<RecursiveSalesBonusBuilder>()
            //    .ToConstructor((x) => new RecursiveSalesBonusBuilder(new RankType[] { RankType.DD, RankType.TM },
            //        DI.Current.Get<CareerManagement>()));

            kernal.Bind<IRecursiveSalesBonus>().To<FinanCitySalesBonus>();
            kernal.Bind<Lead>().To<Lead>();
            kernal.Bind<LeadManagementBuilder>().To<LeadManagementBuilder>();
            kernal.Bind<ILeadManagement>().To<LeadPromotion>();
            kernal.Bind<ILeadManagement>().To<LeadDePromotion>();
            kernal.Bind<OverridingProfileCollection>().To<OverridingProfileCollection>();
            kernal.Bind<OverridingProfileItem>().To<OverridingProfileItem>();
            kernal.Bind<Chatting>().To<Chatting>();
            kernal.Bind<Notification>().To<Notification>();
            kernal.Bind<SalesInvoiceLog>().To<SalesInvoiceLog>();
            kernal.Bind<FollowUp>().To<FollowUp>();

            kernal.Bind<ReferralSalesNotificationService>().To<ReferralSalesNotificationService>();
            kernal.Bind<SalesMortgageNotificationService>().To<SalesMortgageNotificationService>();
            kernal.Bind<LeadFollowUpNotificationService>().To<LeadFollowUpNotificationService>();
            kernal.Bind<RemarkNotificationService>().To<RemarkNotificationService>();
        }
    }
}
