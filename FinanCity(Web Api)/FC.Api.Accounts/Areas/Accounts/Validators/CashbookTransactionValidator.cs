namespace FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators
{
    using FC.Core;
    using FluentValidation;
    using FC.Api.AccountAreas.Areas.Accounts.Models;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using Ninject;
    using System.Collections.Generic;
    using System.Linq;

    public class CashbookTransactionValidator
        : AbstractValidator<CashbookTransactionModel>
    {
        private IEnumerable<CashBookTransactionEntity> _originSource;
        private CashbookSetup _cashbook;
        private CashBookTransaction _cashbookTransaction;
        private AccountSetup _accountSetup;

        public CashbookTransactionValidator()
        {
            RuleFor(x => x).Custom((x, context) =>
            {
                FakeModel(x);
                return;
            });

            RuleFor(x => x).Custom((d, context) =>
            {
                if (DateEmpty())
                    context.AddFailure($@"'Transaction Date' should not be empty.");

                if (CashbookEmpty())
                    context.AddFailure($@"'Cashbook' should not be empty.");

                if (IsTransactionLineTotalGreaterThanZero())
                    context.AddFailure($@"'Transaction' should not be empty.");

                if (IsTransactionAccountEmpty())
                    context.AddFailure($@"'Account Code' should not be empty.");

                var accountCode = "";
                if (HasCashbookMoreThanOne(ref accountCode))
                    context.AddFailure($@"{accountCode} is concerning with the cashbook accounts 
                       that You should not select any cashbook account in this transaction.");
            });
        }

        private bool CashbookEmpty()
        {
            return _originSource.Where(x => string.IsNullOrWhiteSpace(x.CBSK)).Count() > 0;
        }

        private bool DateEmpty()
        {
            return _originSource.Where(x => x.TransactionDate == null).Count() > 0;
        }

        private bool IsTransactionLineTotalGreaterThanZero()
        {
            if (_originSource.Count() <= 0)
            {
                return true;
            }
            return false;
        }

        private bool IsTransactionAccountEmpty()
        {
            foreach (var v in _originSource)
            {
                if (string.IsNullOrWhiteSpace(v.AccSK))
                {
                    return true;
                }
            }
            return false;
        }

        private void FakeModel(CashbookTransactionModel model)
        {
            _cashbookTransaction = DI.Current.Get<CashBookTransaction>();
            _accountSetup = DI.Current.Get<AccountSetup>();
            _cashbook = DI.Current.Get<CashbookSetup>();

            var collector = DI.Current.Get<KendoGridDataCollector<CashBookTransactionEntity>>();
            var parentId = _cashbookTransaction.GetParentId(
                updatedItems: model.updatedItems,
                deletedItems: model.deletedItems);
         
            _originSource = collector.SetKeys(k => k.Add(x => x.SysKey))
                .SetOriginSource(_cashbookTransaction.Finds(filter: f => f.ParentId == parentId).ToList())
                .SetInsertSource(model.insertedItems.ToList())
                .SetUpdateSource(model.updatedItems.ToList())
                .SetDeleteSource(model.deletedItems.ToList())
                .GenerateResult();
        }

        private bool HasCashbookMoreThanOne(ref string accountCode)
        {

            var cashbook = _cashbook.Finds();
            var overlaping = cashbook.Select(x => x.AccSK)
                .Intersect(_originSource.Select(x => x.AccSK))
                .ToArray();

            if (cashbook.Select(x => x.AccSK)
                .Intersect(_originSource.Select(x => x.AccSK)).Count() > 0)
            {
                for (int i = 0; i < overlaping.Count(); i++)
                {
                    accountCode += _accountSetup.Finds(filter: f => f.SysKey == overlaping[i]).First().AccCode;
                }
                return true;
            }

            return false;
        }
    }
}