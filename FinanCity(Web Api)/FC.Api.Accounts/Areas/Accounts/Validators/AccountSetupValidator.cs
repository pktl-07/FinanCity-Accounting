namespace FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators
{
    using FluentValidation;
    using FC.Domain.Accounts.Entity;

    public class AccountSetupValidator : AbstractValidator<AccountSetupEntity>
    {
        public AccountSetupValidator()
        {
            RuleFor(x => x.AccDescription).NotEmpty();
            RuleFor(x => x.AccCode).NotEmpty();
            RuleFor(x => x.CategorySK).NotEmpty();
        }
    }
}