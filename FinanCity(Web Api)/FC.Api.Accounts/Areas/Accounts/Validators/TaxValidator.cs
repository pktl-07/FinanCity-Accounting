namespace FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators
{
    using FluentValidation;
    using FC.Domain.Accounts.Entity;

    public class TaxValidator : AbstractValidator<TaxEntity>
    {
        #region Ctor

        public TaxValidator()
        {
            RuleFor(x => x.Description).NotEmpty();

            RuleFor(x => x.InputTaxSK).NotEmpty();

            RuleFor(x => x.OutputTaxSK).NotEmpty();
        }

        #endregion
    }
}