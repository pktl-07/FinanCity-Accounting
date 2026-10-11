namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Sales.Models;

    public class SalespersonManagementValidator : AbstractValidator<SalespersonManagementModel>
    {
        #region Ctor

        public SalespersonManagementValidator()
        {
            RuleFor(x => x.Person.Name).NotEmpty();
            RuleFor(x => x.Person.NRIC).NotEmpty();
            RuleFor(x => x.Contact.Email).EmailAddress();
            RuleFor(x => x.Employment.SchemeDetailSysKey).NotEmpty().WithMessage("'Commission Scheme' should not be empty.");
            RuleFor(x => x.EmploymentHistory.DateJoin).NotEmpty();
        }

        #endregion
    }
}