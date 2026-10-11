namespace FC.Api.SalesAreas.Areas.Validators
{
    using FC.Domain.Sales.Entity;
    using FluentValidation;

    public class CurrencyRateValidator : AbstractValidator<CurrencyRateEntity>
    {
        #region Ctor

        public CurrencyRateValidator()
        {
            RuleFor(x => x.SysKey).NotEmpty().WithMessage("'Currency Code' is required");
            RuleFor(x => x.Symbol).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Rate).GreaterThan(0);

        }

        #endregion
    }
}