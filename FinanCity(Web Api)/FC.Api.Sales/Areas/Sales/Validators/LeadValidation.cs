namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Sales.Models;

    public class LeadValidation : AbstractValidator<LeadCreateModal>
    {
        #region Ctor

        public LeadValidation()
        {
            RuleFor(x => x.Person.Name).NotEmpty();
            RuleFor(x => x.Contact.Email).EmailAddress().WithMessage("'Email' is incorrect format");

            RuleFor(x => x.Lead.Source).NotEmpty();
            RuleFor(x => x.Lead.PropertyTypeSK).NotEmpty();
            RuleFor(x => x.Lead.SalesActivity).NotEmpty();

            RuleFor(x => x.FollowUp.StartDate).NotEmpty();
            RuleFor(x => x.FollowUp.StartTime).NotEmpty();
        }

        #endregion
    }
}