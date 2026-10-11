namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Domain.Sales.Entity;

    public class BankBestRateValidator : AbstractValidator<BankBestRateEntity>
    {
        public BankBestRateValidator()
        {
            RuleFor(x => x.EffectiveDate).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Rate).GreaterThan(0);
        }
    }
}