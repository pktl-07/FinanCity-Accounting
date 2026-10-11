namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Sales.Models;
   
    public class ExternalSalespersonValidator : AbstractValidator<PersonAndContactModel>
    {
        #region Ctor

        public ExternalSalespersonValidator()
        {
            RuleFor(x => x.Person.Name).NotEmpty();
            RuleFor(x => x.Person.NRIC).NotEmpty();
            RuleFor(x => x.Contact.Email).EmailAddress().WithMessage("'Email' is incorrect format");
        }

        #endregion
    }
}