namespace FC.Domain.Sales.Infrastructure
{
    public interface ICommissionBaseHelperFactory
    {
        decimal GetSalesCommissionToDate();
        decimal GetSalesCommissionToYear();
        decimal FindGrossToNextPromote();
        decimal GetSalesValueToDate();
        decimal GetSalesValueToYear();
    }
}
