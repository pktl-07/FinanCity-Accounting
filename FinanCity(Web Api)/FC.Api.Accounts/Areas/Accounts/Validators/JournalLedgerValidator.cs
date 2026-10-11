namespace FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators
{
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using Ninject;
    using FluentValidation;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
  
    public class JournalLedgerValidator : AbstractValidator<IEnumerable<BTableEntity>>
    {
        #region Ctor

        public JournalLedgerValidator()
        {
            RuleFor(x => x).Custom((d, context) =>
            {
                if (d.Count() == 0)
                {
                    context.AddFailure($@"Inconsistant posting records.");
                }

                if (d.GroupBy(x => new
                {
                    x.BatchNo,
                    x.ParentId,
                    x.PostDate,
                    x.ModuleCode,
                    x.EntryDescription,
                    x.SecondRef,
                    x.ReversalRefNo,
                    x.IsAutoOpening,
                    x.TransTypeDescription,
                    x.TransType,
                    x.CURRATE,
                    x.CURRSK,
                    x.CURROP
                }).Count() > 1)
                {
                    context.AddFailure($@"Inconsistant posting records.");
                }

                if (d.Where(x => x.PostDate == null).Count() > 0)
                {
                    context.AddFailure($@"'Post Date' must not be empty.");
                }

                if (d.Where(x => x.ModuleCode == null).Count() > 0)
                {
                    context.AddFailure($@"'Module Code' must not be empty.");
                }

                if (d.Where(x => string.IsNullOrWhiteSpace(x.CURRSK)).Count() > 0)
                {
                    context.AddFailure($@"'CURRSK' must not be empty.");
                }

                if (d.Where(x => string.IsNullOrWhiteSpace(x.CURROP)).Count() > 0)
                {
                    context.AddFailure($@"'CURROP' must not be empty.");
                }

                if (d.Where(x => x.CURRATE == 0).Count() > 0)
                {
                    context.AddFailure($@"'CURRATE' must be greater than 0.");
                }

                if (d.Sum(x => x.TransactionAmount.ToRoundUp()) != 0)
                {
                    context.AddFailure($@"'Debit' and 'Credit' amount must be balance.");
                }

                if (d.Where(x => string.IsNullOrWhiteSpace(x.AccSK)).Count() > 0)
                {
                    context.AddFailure("'Account Code' must not be empty.");
                }

                var accountCode = "";
                if (ContainCashbookTransaction(ref accountCode, d))
                {
                    context.AddFailure($@"'{accountCode}' are concerning with the cashbook accounts. 
                        You should do entry in the Cashbook Transaction.");
                }

                if (d.Count() > 0 && !IsValidEntry(d))
                {
                    context.AddFailure($@"'{GeneralUtility.ConvertDisplayDateFormat(d.First().PostDate)}' 
                       is not valid date for the current financial year. ");
                }

                var defaultAccount = "";
                if (d.Count() > 0 && UseDefaulAccount(accountCode: ref defaultAccount, source: d))
                {
                    context.AddFailure($@"'{defaultAccount}' is reserved account for default such as 'Sales Transaction' or 'Sales Receivable' ect. 
                      The reserved account are not allowed to post in General Ledger.");
                }
            });
        }

        private bool IsValidEntry(IEnumerable<BTableEntity> source)
        {
            try
            {
                var batchModule = DI.Current.Get<BatchModule>();
                var date = source.First().PostDate;
                var module = source.First().ModuleCode;

                return batchModule.IsValidEntry(date.Value, module);
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
       }

        private bool ContainCashbookTransaction(ref string accountCode,
            IEnumerable<BTableEntity> source)
        {
            var cashBook = DI.Current.Get<CashbookSetup>().Finds();
            var accountSetup = DI.Current.Get<AccountSetup>();

            var overlaping = cashBook.Select(x => x.AccSK)
                .Intersect(source.Select(x => x.AccSK)).ToArray();

            if (overlaping.Length > 0)
            {
                for (int i = 0; i < overlaping.Count(); i++)
                {
                    var accsk = overlaping[i];
                    accountCode += accountSetup.Finds(filter: f => f.SysKey == accsk).Single().AccCode;

                    if (i < (overlaping.Count() - 1))
                    {
                        accountCode += ", ";
                    }
                }
                return true;
            }

            return false;
        }

        private bool UseDefaulAccount(ref string accountCode ,
            IEnumerable<BTableEntity> source)
        {
            try
            {
                var app = DI.Current.Get<IEssAppRepository>();
                var accdic = app.DefaultAccountingCode.ToJournalPreventAccountPostingDictionary();
                var intersect = source.Select(x => x.AccSK)
                    .Intersect(accdic.Select(x => x.Key));
                var count = intersect.Count();

                if (count > 0)
                {
                    var key = intersect.First();
                    accountCode = accdic[key];
                }

                return count > 0;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
 
        #endregion
    }

    public class JournalLedgerReverseValidator : AbstractValidator<IEnumerable<BTableEntity>>
    {
        public JournalLedgerReverseValidator()
        {
            RuleFor(x => x).Custom((d, context) =>
            {
                if (d.First().IsAutoOpening)
                {
                    context.AddFailure($@"The opening entry accounts should not be reversed or void.");
                }                
            });
        }
   }
}