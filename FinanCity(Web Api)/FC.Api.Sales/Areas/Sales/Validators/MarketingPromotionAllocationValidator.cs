namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Domain.Sales.Entity;
 
    public class MarketingPromotionAllocationValidator : AbstractValidator<AllocationEntity>
    {
        #region Ctor

        public MarketingPromotionAllocationValidator()
        {
            RuleFor(x => x.HeaderSK).NotEmpty();
            RuleFor(x => x.SPSK).NotEmpty();
        }

        #endregion
    }
}