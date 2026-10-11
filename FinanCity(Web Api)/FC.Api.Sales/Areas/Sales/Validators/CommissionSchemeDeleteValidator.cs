namespace FC.Api.SalesAreas.Areas.Validators
{
    using System.Linq;
    using FluentValidation;
    using FC.Core;
    using Ninject;
    using FC.Domain.Sales.Business;

    public class CommissionSchemeDeleteValidator : AbstractValidator<string>
    {
        #region Fields

        private CommissionStructure _commStructure = DI.Current.Get<CommissionStructure>();

        #endregion

        #region Ctor

        public CommissionSchemeDeleteValidator()
        {
            RuleFor(x => x).Custom((d, context) =>
            {
            });
        }

        #endregion
    }
}