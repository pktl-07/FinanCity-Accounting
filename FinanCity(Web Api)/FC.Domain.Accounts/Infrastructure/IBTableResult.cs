namespace FC.Domain.Accounts
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Core.Data;
    using Ninject;
    using FC.Core.Infrastructure;
    using FC.Core.Infrastructure.Implements.RecordIndexer;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;

    public abstract class IBTableResult<T> where T : IEntity
    {
        #region Fields

        private AccountingPeriod _accountingPeriod;
        private RecordIndexerFactory _recordIndexer;
        private BTable _business;
        private Tax _tax;
        private IMultiCurrencyConverter _currencyConverter;
        private IEssAppRepository _app;
       
        public event EventHandler<JournalPostEventArgs> OnJournalPosted;

        #endregion

        #region Ctor

        public IBTableResult()
        {
            _currencyConverter = DI.Current.Get<IMultiCurrencyConverter>();
            _business = DI.Current.Get<BTable>();
            _recordIndexer = DI.Current.Get<RecordIndexerFactory>();
            _accountingPeriod = DI.Current.Get<AccountingPeriod>();
            _tax = DI.Current.Get<Tax>();
            _app = DI.Current.Get<IEssAppRepository>();
        }

        #endregion

        #region Implements

        public virtual void ExcuteReverse(string key,
           int postDate,
           string module)
        {
            try
            {
                Guard.IsNotNull(key, "key");
                Guard.IsNotZeroOrNegative(postDate, "postDate");
                Guard.IsNotNull(module, "module");

                var d = _business.Finds(filter: f => f.SourceHeaderTransSK == key &&
                    f.PostDate == postDate &&
                    f.RecordStatus == (int)RecordStatus.Post &&
                    f.ModuleCode == module);

                if (d.Count() > 0)
                    _business.ReverseAsTransaction(moduleCode: module,
                       postDate: postDate,
                       parentId: d.First().ParentId,
                       transType: (TransType)d.First().TransType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void ExcuteVoid(string key,
            int postDate,
            string module)
        {
            try
            {
                Guard.IsNotNull(key, "key");
                Guard.IsNotZeroOrNegative(postDate, "postDate");
                Guard.IsNotNull(module, "module");

                var d = _business.Finds(filter: f => f.SourceHeaderTransSK == key &&
                    f.PostDate == postDate &&
                    f.RecordStatus == (int)RecordStatus.Post &&
                    f.ModuleCode == module);

                if (d.Count() > 0)
                    _business.VoidAsTransaction(moduleCode: module,
                        parentId: d.First().ParentId,
                        postDate: postDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void ExcutePost(IEnumerable<BTableEntity> bTableEntyList)
        {
            if (bTableEntyList.Count() > 0)
            {
                var setting = _recordIndexer.Generate(TransType.Journal, bTableEntyList.First());
                EventHandler<JournalPostEventArgs> idHandler = OnJournalPosted;

                //Raise Event
                if (idHandler != null)
                    idHandler(this, new JournalPostEventArgs { JournalNo = setting.R1 });

                // Set Journal References
                bTableEntyList.Each(bTable =>
                {
                    bTable.R1 = setting.R1;
                    bTable.Prefix = setting.Prefix;
                    bTable.Postfix = setting.Postfix;
                    bTable.VoucherId = setting.VoucherId;
                    bTable.RecordStatus = (int)RecordStatus.Post;
                    //convert multi currency
                    _currencyConverter.ToMultiCurrency(bTable);
                });

                //string time = GeneralUtility.ServerTime;
                // Save and post the transaction.
                _business.InsertAsTransaction(data: bTableEntyList,
                    moduleCode: bTableEntyList.First().ModuleCode,
                    postDate: bTableEntyList.First().PostDate,
                    transType: (TransType)bTableEntyList.First().TransType,
                    recordStatus: (int)RecordStatus.Post);
            }
        }

        public abstract IEnumerable<BTableEntity> Convert(IEnumerable<T> entityList, object parameters);

        public string GetYCPC(int? date)
        {
            var yc = _accountingPeriod.Finds(x => x.Date1 <= date.Value && x.Date2 >= date.Value).Last();
            return $"{yc.YC}{yc.PC}";
        }

        public string GetYC(int? date)
        {
            return GetYCPC(date).Substring(0, 2);
        }

        public decimal ConvertTaxAmount(char sign, decimal taxAmt, string taxSK)
        {
            var tax = _tax.Finds(filter: f => f.SysKey == taxSK).Single();
            if (tax.TaxType == (int)TaxType.Purchase
                || tax.TaxType == (int)TaxType.CommonModuleBase)
            {
                if (sign == '-')
                    return Math.Abs(taxAmt) * (-1);
                else if (sign == '+')
                    return Math.Abs(taxAmt);
            }
            else if (tax.TaxType == (int)TaxType.Supply)
            {
                if (sign == '+')
                    return Math.Abs(taxAmt) * (-1);
                else if (sign == '-')
                    return Math.Abs(taxAmt);
            }

            return Math.Abs(taxAmt);
        }

        public decimal RealizeCurrencyGainL(decimal total,
          string receiptCurrSK, decimal receiptRate, string receiptOp,
          string invoiceCurrSK, decimal invoiceRate, string invoiceOp,
          string originCurrSK, decimal originRate, string originOp)
        {
            Func<string, decimal, decimal> convertRate = (op, rate) =>
            {
                if (op == "/")
                    return 1 / rate;
                else
                    return 1 * rate;
            };

            var receiptBaseSimpleRate = convertRate(receiptOp, receiptRate);
            var invoiceBaseSimpleRate = convertRate(invoiceOp, invoiceRate);
            var originBaseSimpleRate = convertRate(originOp, originRate);

            var invoiceRatio = 0.0M;
            var receiptRatio = 0.0M;

            if (receiptCurrSK == originCurrSK && receiptRate != originRate)
            {
                receiptRatio = total * (receiptBaseSimpleRate / invoiceBaseSimpleRate);
                invoiceRatio = total * originBaseSimpleRate;

                return receiptRatio - invoiceRatio;
            }
            else if (invoiceCurrSK == originCurrSK && invoiceRate != originRate)
            {
                if (receiptCurrSK == _app.CURRSK)
                {
                    receiptRatio = total; // no need to convert to base rate cos it is alrady a base currency.
                    invoiceRatio = total * (invoiceRate / originRate); // no need to convert to base rate cos it is alrady a base currency.
                }
                else if (receiptCurrSK == invoiceCurrSK)
                {
                    receiptRatio = _currencyConverter.ToBaseCurrency(total * (invoiceRate / receiptRate), receiptRate, receiptOp == "*" ? Operator.Multiple : Operator.Divider); // convert to base rate at time of receipt.*/
                    invoiceRatio = _currencyConverter.ToBaseCurrency(total, originRate, originOp == "*" ? Operator.Multiple : Operator.Divider); // convert to base rate at the time of invoice.
                }
                else if (receiptCurrSK != invoiceCurrSK)
                {
                    receiptRatio = _currencyConverter.ToBaseCurrency(total * (receiptBaseSimpleRate / invoiceBaseSimpleRate), originRate, originOp == "*" ? Operator.Multiple : Operator.Divider); // using this formular will outcome the invoice rate then need to convert to the base rate.
                    invoiceRatio = _currencyConverter.ToBaseCurrency(total, receiptRate, receiptOp == "*" ? Operator.Multiple : Operator.Divider);
                }
            }

            return invoiceRatio - receiptRatio;
        }

        #endregion
    }

    public class JournalPostEventArgs : EventArgs
    {
        public string JournalNo { get; set; }
    }
}
