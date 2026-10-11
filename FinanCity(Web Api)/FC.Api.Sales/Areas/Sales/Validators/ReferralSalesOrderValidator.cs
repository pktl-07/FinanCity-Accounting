namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Sales.Models;

    public class ReferralSalesOrderValidator : AbstractValidator<ReferralSalesOrderModel>
    {
        #region Ctor

        public ReferralSalesOrderValidator()
        {
            RuleFor(x => x.PropertyTransaction.CURRSK).NotEmpty().WithMessage("'Currency Code' should not be empty.");
            RuleFor(x => x.PropertyTransaction.CURRATE).GreaterThan(0).WithMessage("'Currency Rate' should be greater than 0.");
            RuleFor(x => x.PropertyTransaction.TransactionDate).NotEmpty();
            RuleFor(x => x.PropertyTransaction.GrossCommission).GreaterThan(0).WithMessage("'Gross Commission' should be greater than 0.");
            RuleFor(x => x.Client.ClientSK).NotEmpty().WithMessage("'Client' should not be empty.");
            RuleFor(x => x.Internal.SPSK).NotEmpty().WithMessage("'Salesperson' should not be empty.");
            RuleFor(x => x).Custom((d, context) =>
            {
                if (d.PropertyTransaction.BillToCoBroke == true
                && (d.PropertyTransaction.ClientTransType == null || string.IsNullOrWhiteSpace(d.PropertyTransaction.BankerSK)))
                {
                    context.AddFailure("The information is not completed when you are selecting Bill To Bank");
                }
            });
        }

        #endregion
    }
}