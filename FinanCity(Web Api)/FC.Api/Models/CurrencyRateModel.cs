using FC.Domain.Sales.Entity;

namespace FC.Api.Models
{
    public class CurrencyRateModel
    {
        public CurrencyEntity Currency { get; set; }

        public CurrencyRateEntity CurrencyRate { get; set; }
    }
}