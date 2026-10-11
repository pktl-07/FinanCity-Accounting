namespace FC.Api.SalesAreas.Areas.Validators
{
    using FluentValidation;
    using FC.Domain.Sales.Entity;

    public class PromotionItemValidator : AbstractValidator<MarketingItemEntity>
    {
        #region Ctor

        public PromotionItemValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
        }

        #endregion
    }
}