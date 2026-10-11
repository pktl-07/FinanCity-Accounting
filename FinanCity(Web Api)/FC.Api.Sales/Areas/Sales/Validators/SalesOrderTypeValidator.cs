namespace FC.Api.SalesAreas.Areas.Validators
{
    using FC.Domain.Sales.Entity;
    using FluentValidation;

    public class SalesOrderTypeValidator : AbstractValidator<SalesOrderTypeEntity>
    {
        #region Ctor

        public SalesOrderTypeValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.IncomeAccountSK).NotEmpty();
        }

        #endregion
    }
}