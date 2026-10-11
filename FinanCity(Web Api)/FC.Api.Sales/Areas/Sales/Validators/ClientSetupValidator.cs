namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Accounts.Entity;

    public class ClientSetupValidator : AbstractValidator<PersonAndContactModel>
    {
        public ClientSetupValidator()
        {
            RuleFor(x => x.Person.R1).NotEmpty();

            RuleFor(x => x.Person.Name).NotEmpty();

            RuleFor(x => x.Contact.Email).EmailAddress().WithMessage("'Email' is incorrect format");

            RuleFor(x => x.Person.NRIC).NotEmpty();
        }
    }
}