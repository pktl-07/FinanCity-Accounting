namespace FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators
{
    using FluentValidation;
    using FC.Domain.Accounts.Entity;

    public class CashbookTransferValidator : AbstractValidator<CashBookTransferEntity>
    {
        public CashbookTransferValidator()
        {
            RuleFor(x => x.FromCBSysKey).NotEmpty();
            RuleFor(x => x.ToCBSysKey).NotEmpty();
            RuleFor(x => x.FromTransactionAmount).NotEqual(0);
            RuleFor(x => x.ToTransactionAmount).NotEqual(0);
            RuleFor(x => x.TransactionDate).NotEmpty();
            RuleFor(x => x.FromCBSysKey).NotEqual(x => x.ToCBSysKey).WithMessage("'From Cashbook and To Cashbook' should not be the same");
        }
    }
}