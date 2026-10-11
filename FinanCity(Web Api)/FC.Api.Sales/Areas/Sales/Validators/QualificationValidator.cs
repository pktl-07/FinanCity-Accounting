namespace FC.Api.SalesAreas.Areas.Validators
{
    using FC.Domain.Sales.Entity;
    using FluentValidation;

    public class QualificationValidator : AbstractValidator<QualificationEntity>
    {
        #region Ctor

        public QualificationValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
        }

        #endregion
    }
}