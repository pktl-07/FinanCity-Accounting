namespace FC.Core
{
    using FC.Core.Data.SQL;
    using Ninject;
    using FC.Core.Infrastructure.Implements;
    using FC.Core.Infrastructure.Implements.RecordIndexer;
    using FC.Core.Infrastructure.Implements.DataCollector;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Core.Infrastructure.CrystalDocument;
    using FC.Core.Infrastructure.ConfigurationManager;

    public class DependencyContainer : DI
    {
        public override void RegisterService(IKernel kernal)
        {
            //here I want to override the existing insert and update features.
            kernal.Rebind<Insert>().To<CustomizeInsertBuilder>();
            kernal.Rebind<Update>().To<CustomizeUpdateBuilder>();
            kernal.Rebind<Delete>().To<CustomizeDeleteBuilder>();
            kernal.Bind<RecordIndexerFactory>().To<RecordIndexerFactory>();
            kernal.Bind<IEntityAnalyzerBuilder>().To<EntityAnalyzerBuilder>();
            kernal.Bind<ExpressionAnalyzer>().To<ExpressionAnalyzer>();
            kernal.Bind<IReferenceFieldAnalyzer>().To<RelationConstaint>();
            kernal.Bind(typeof(IUniqueFieldAnalyzer<>)).To(typeof(UniqueConstraint<>));
            kernal.Bind(typeof(KendoGridDataCollector<>)).To(typeof(KendoGridDataCollector<>));
            kernal.Bind(typeof(ICrystalReporter)).To(typeof(FinanCityReportDocument));
            kernal.Bind<DownloadResult>().To<DownloadResult>();
            kernal.Bind(typeof(OpeningAndClosingBalanceCollection<>)).To(typeof(OpeningAndClosingBalanceCollection<>));
            kernal.Bind<IMultiCurrencyConverter>().To<MultiCurrencyConverter>();
            kernal.Bind<FinanCityConfig>().To<FinanCityConfig>();
            kernal.Bind<IFinanCityIdentity>().To<FinanCityIdentity>();
            kernal.Bind<KendoGridFilterTransalator>().To<KendoGridFilterTransalator>();
        }
    }
}
