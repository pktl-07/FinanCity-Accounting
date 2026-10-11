namespace FC.Api.SalesAreas.Areas.Validators
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core;
    using Ninject;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
    
    public class PropertyTransactionValidator : AbstractValidator<PropertyTransactionCreateOrUpdateModel>
    {
        #region Fields

        private IEnumerable<PTOClientEntity> _clients;
        private IEnumerable<PropertyTransactionDetailEntity> _externals;
        private IEnumerable<PropertyTransactionDetailEntity> _internals;
        private PropertyTransactionCreateOrUpdateModel _model;

        private Person _person;
        private Employment _employment;
        private CommissionStructure _commissionStructure;
        private CommissionStructureDetail _commissionStructureDetail;

        #endregion

        #region Ctor

        public PropertyTransactionValidator()
        {
            RuleFor(x => x).Custom((x, context) =>
            {
                FakeModel(x);
                return;
            });

            RuleFor(x => x.Property.CURRATE).GreaterThan(0);
            RuleFor(x => x.Property.TransactedPrice).GreaterThan(0);
            RuleFor(x => x.Property.TransactionDate).NotEmpty();
      
            #region Client Validation       

            RuleFor(x => x).Custom((d, context) =>
            {
                if (_clients.Count() == 0)
                    context.AddFailure("'Çlients' should not be empty.");

                if (_clients.Where(x => x.IsMain == true).Count() == 0)
                    context.AddFailure("'Clients' at least one should be main client.");

                if (_clients.Where(x => x.IsMain == true).Count() > 1)
                    context.AddFailure("'Clients' should not be more than one main client.");

                if (_clients.GroupBy(x => x.ClientSK).Where(g => g.Count() > 1).Count() > 0)
                    context.AddFailure("'Clients' should not be duplicated.");

                if (_clients.Where(x => string.IsNullOrWhiteSpace(x.ClientSK)).Count() > 0)
                    context.AddFailure("'Clients' name should not be empty.");

                if (_model.Property.BillToCoBroke == true && (_model.Property.ClientTransType == null || string.IsNullOrWhiteSpace(_model.Property.BankerSK)))
                    context.AddFailure("The information is not completed when you are selecting Bill To Bank");
            });

            #endregion

            #region Internals

            RuleFor(x => x).Custom((System.Action<PropertyTransactionCreateOrUpdateModel, FluentValidation.Validators.CustomContext>)((d, context) =>
            {
                if (this._internals.Count() == 0)
                    context.AddFailure("'Salesperson' should not be empty.");

                if (this._internals.Where(x => x.IsMain == true).Count() == 0)
                    context.AddFailure("'Salesperson' at least one should be main salesperson.");

                if (this._internals.Where(x => x.IsMain == true).Count() > 1)
                    context.AddFailure("'Main salesperson' should not be more than one main client.");

                if (_internals.GroupBy(x => x.SPSK).Where(g => g.Count() > 1).Count() > 0)
                    context.AddFailure("'Salesperson' should not be duplicated.");

                if (_internals.Where(x => string.IsNullOrWhiteSpace(x.SPSK)).Count() > 0)
                    context.AddFailure("'Salesperson' name should not be empty");

                if (_internals.Where(x => x.GrossComm <= 0).Count() > 0)
                    context.AddFailure("'Salesperson' Gross Commission should be greater than 0");

                FC.Core.EnumerableExtensions.Each(_internals, (System.Action<PropertyTransactionDetailEntity>)(x =>
                 {
                     var e = _employment.Finds(filter: f => f.SysKey == x.SPSK).Single();
                     var dt = _commissionStructureDetail.Finds(filter: f => f.SysKey == e.SchemeDetailSysKey).Single();
                     var s = _commissionStructure.Finds(filter: f => f.SysKey == dt.ParentSK).Single();
                     var scheme = _commissionStructureDetail.Finds(filter: f => f.SysKey == x.SchemeDetailSK).Single();
                     if (s.BaseHeaderSK == "01")
                     {
                         if (scheme.Type == (int)FC.Core.CommissonStructureType.Tier)
                         {
                             var sp = _person.Finds(filter: f => f.SysKey == x.SPSK).Single();
                             context.AddFailure($@"'{sp.Name}' must not be {FC.Core.CommissonStructureType.Tier.ToDescription()} type.");
                         }
                     }
                     else if (s.BaseHeaderSK == "02")
                     {
                         if (scheme.Type == (int)FC.Core.CommissonStructureType.CompanyLeads ||
                          scheme.Type == (int)FC.Core.CommissonStructureType.PersonalLeads ||
                          scheme.Type == (int)FC.Core.CommissonStructureType.RESLeads)
                         {
                             var sp = _person.Finds(filter: f => f.SysKey == x.SPSK).Single();
                             context.AddFailure($@"'{sp.Name}' must not be {((FC.Core.CommissonStructureType)scheme.Type).ToDescription()} type.");
                         }
                     }
                 }));
            }));

            #endregion

            #region Externals

            RuleFor(x => x).Custom((d, context) =>
            {
                if (_externals.GroupBy(x => x.SPSK).Where(g => g.Count() > 1).Count() > 0)
                    context.AddFailure("'External Salesperson' should not be duplicated.");

                if (_externals.Where(x => string.IsNullOrWhiteSpace(x.SPSK)).Count() > 0)
                    context.AddFailure("'External Salesperson' name should not be empty");

                if (_externals.Where(x => x.GrossComm <= 0).Count() > 0)
                    context.AddFailure("'External Salesperson' Gross Commission should be greater than 0");
            });

            #endregion

            #region Transaction

            RuleFor(x => x.Property.BillToCoBroke).Equal(true).WithMessage("'Bill To Client' must be checked");
           
            RuleFor(x => x).Custom((d, context) =>
            {
                if (_model.Property.GrossCommission.ToRoundUp() != _internals.Sum(x => x.GrossComm.ToRoundUp()) + _externals.Sum(x => x.GrossComm.ToRoundUp()))
                    context.AddFailure("'Agency Gross Commission' must be equal to total of Salesperson's Gross Commission.");
            });

            #endregion
        }

        #endregion

        #region Utilities

        private void FakeModel(PropertyTransactionCreateOrUpdateModel model)
        {
            var saleOrderDetail = DI.Current.Get<PropertyTransactionDetail>();
            var clientDetail = DI.Current.Get<PTOClient>();
            var clientCollector = DI.Current.Get<KendoGridDataCollector<PTOClientEntity>>();
            var salespersonCollector = DI.Current.Get<KendoGridDataCollector<PropertyTransactionDetailEntity>>();

            _person = DI.Current.Get<Person>();
            _employment = DI.Current.Get<Employment>();
            _commissionStructure = DI.Current.Get<CommissionStructure>();
            _commissionStructureDetail = DI.Current.Get<CommissionStructureDetail>();

            _model = model;

            _internals = salespersonCollector.SetKeys(k => k.Add(x => x.SysKey))
                .SetOriginSource(saleOrderDetail.Finds(filter: f => f.PTOSK == model.Property.SysKey && (f.TransType >= (int)TransType.InternalSalesperson && f.TransType <= (int)TransType.MainSalesperson)).ToList())
                .SetInsertSource(model.InternalInsertedItems.ToList())
                .SetUpdateSource(model.InternalUpdatedItems.ToList())
                .SetDeleteSource(model.InternalDeletedItems.ToList())
                .GenerateResult();

            _externals = salespersonCollector.SetKeys(k => k.Add(x => x.SysKey))
                .SetOriginSource(saleOrderDetail.Finds(filter: f => f.PTOSK == model.Property.SysKey && (f.TransType == (int)TransType.ExternalSalesPerson)).ToList())
                .SetInsertSource(model.ExternalInsertedItems.ToList())
                .SetUpdateSource(model.ExternalUpdatedItems.ToList())
                .SetDeleteSource(model.ExternalDeletedItems.ToList())
                .GenerateResult();

            _clients = clientCollector.SetKeys(k => k.Add(x => x.SysKey))
                .SetOriginSource(clientDetail.Finds(filter: f => f.PTOSK == model.Property.SysKey).ToList())
                .SetInsertSource(model.ClientInsertedItems.ToList())
                .SetUpdateSource(model.ClientUpdatedItems.ToList())
                .SetDeleteSource(model.ClientDeletedItems.ToList())
                .GenerateResult();
        }

        #endregion
    }
}