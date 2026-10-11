namespace FC.Domain.Sales.Infrastructure
{
    public abstract class PromotionSchmeFactoryBase
    {
        public abstract void PopuplateScheme();

        public abstract bool HasDifferentScheme { get; }

        public abstract object GetLatestScheme();

        public abstract string GetLatestBatchNo();
    }
}
