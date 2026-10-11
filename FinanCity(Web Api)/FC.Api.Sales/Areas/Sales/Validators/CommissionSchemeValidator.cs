namespace FC.Api.SalesAreas.Areas.Validators
{
    using System.Linq;
    using System.Collections.Generic;
    using FluentValidation;
    using FC.Core;
    using Ninject;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Sales.Business;
  
    public class CommissionSchemeValidator : AbstractValidator<CommissionSchemeCrateOrUpdateModel>
    {
        #region Fields

        private CommissionSchemeCrateOrUpdateModel _model;
        private IEnumerable<CommissionStructureDetailEntity> _commDetailList;

        #endregion

        #region Ctor

        public CommissionSchemeValidator()
        {
            RuleFor(x => x).Custom((x, context) =>
            {
                FakeModel(x);
                return;
            });

            RuleFor(x => x.CommissionScheme.SchemeName).NotEmpty();

            RuleFor(x => x).Custom((d, context) =>
            {
                _commDetailList.Each(data =>
                {
                    if (data.AgencyPercent + data.AssociatedPercent != 100)
                    {
                        context.AddFailure($@"{data.Designation}'s total commission percentage must equal to {100}%");
                    }
                });
            });
        }

        #endregion

        #region Utilities

        private void FakeModel(CommissionSchemeCrateOrUpdateModel model)
        {
            var commDetail = DI.Current.Get<CommissionStructureDetail>();
            var commissionCollector = DI.Current.Get<KendoGridDataCollector<CommissionStructureDetailEntity>>();

            _model = model;

            _commDetailList = commissionCollector.SetKeys(k => k.Add(x => x.SysKey))
                .SetOriginSource(commDetail.Finds(filter: f => f.ParentSK == model.CommissionScheme.SysKey).ToList())
                .SetInsertSource(model.Inserted.ToList())
                .SetUpdateSource(model.Updated.ToList())
                .SetDeleteSource(model.Deleted.ToList())
                .GenerateResult();
        }

        #endregion
    }
}