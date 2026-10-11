namespace FC.Domain.Sales.DTO
{
    using FC.Core.Data;
    using FC.Domain.Sales.Entity;

    public class CurrencyRateDTO : QueryAssert<CurrencyRateDTO>
    {
        #region Ctor

        public CurrencyRateDTO() : base("FC.Biz")
        {
            RuleFor(x => x.SysKey).Reference<CurrencyEntity>(x => x.SysKey);
            RuleFor(x => x.CurrencyCode).Reference<CurrencyEntity>(x => x.Currency_AlphaCode);
            RuleFor(x => x.Country).Reference<CurrencyEntity>(x => x.Country);
            RuleFor(x => x.Operator).Reference<CurrencyRateEntity>(x => x.Operator);
            RuleFor(x => x.Symbol).Reference<CurrencyRateEntity>(x => x.Symbol);
            RuleFor(x => x.Rate).Reference<CurrencyRateEntity>(x => x.Rate);
        }

        #endregion

        #region Properties

        public string SysKey { get; set; }
        public string CurrencyCode { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string Operator { get; set; }
        public string Symbol { get; set; }
        public decimal Rate { get; set; }

        #endregion
    }
}
