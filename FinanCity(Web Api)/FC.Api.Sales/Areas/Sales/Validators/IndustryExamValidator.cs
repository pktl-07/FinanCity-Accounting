namespace FC.Api.SalesAreas.Areas.Validators
{
    using FC.Domain.Sales.Entity;
    using FluentValidation;

    public class IndustryExamValidator : AbstractValidator<IndustryExamEntity>
    {
        #region Ctor

        public IndustryExamValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
        }

        #endregion
    }
}