namespace FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators
{
    using FluentValidation;
    using FC.Domain.Accounts.Entity;

    public class CashbookSetupValidator : AbstractValidator<CashbookSetupEntity>
    {
        public CashbookSetupValidator()
        {
            RuleFor(x => x.CashbookName).NotEmpty();

            RuleFor(x => x.CBType).NotEmpty();

            RuleFor(x => x.Description).NotEmpty();

            RuleFor(x => x.CURRSK).NotEmpty();

            RuleFor(x => x.AccSK).NotEmpty();

            RuleFor(x => x.ForeignExchangeGainSK).NotEmpty();

            RuleFor(x => x.ForeignExchangeGainSK).NotEmpty();
        }
    }
}