namespace FC.Api.SalesAreas.Areas.Sales.Models
{
    using FC.Domain.Sales.Entity;

    public class CurrencyRateModel
    {
        public CurrencyEntity Currency { get; set; }

        public CurrencyRateEntity CurrencyRate { get; set; }
    }
}