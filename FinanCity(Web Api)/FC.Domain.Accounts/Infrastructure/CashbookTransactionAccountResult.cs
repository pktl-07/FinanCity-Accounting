namespace FC.Domain.Accounts.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using Ninject;
    using FC.Core;
    using FC.Core.Infrastructure.Implementation;

    public class CashbookTransactionAccountResult : IBTableResult<CashBookTransactionEntity>
    {
        #region Fields

        private CashbookSetup _cashbookSetup;
        private Tax _tax;

        #endregion

        #region Ctor

        public CashbookTransactionAccountResult(CashbookSetup cashbookSetup,
        Tax tax)
        {
            _cashbookSetup = cashbookSetup;
            _tax = tax;
        }


        #endregion

        #region Methods

        public override IEnumerable<BTableEntity> Convert(IEnumerable<CashBookTransactionEntity> entityList,
          object parameters)
        {
            var bTableEntity = new List<BTableEntity>();
            var cbt = (CashBookTransactionEntity)parameters;

            switch ((TransType)cbt.TransType)
            {
                case TransType.CashBookPayment:

                    #region Prepare Detail Accounts

                    foreach (var v in entityList)
                    {
                        var transactionAmount = FindTransactionAmount(v.Inclusive, v.TaxAmount, v.TransactionAmount);
                        bTableEntity.Add(new BTableEntity
                        {
                            AccSK = v.AccSK,
                            PostDate = v.TransactionDate,
                            TransactionAmount = transactionAmount,
                            IsTaxInclusive = false,
                            EntryDescription = cbt.Description,
                            LineDescription = v.Memo,
                            ParentId = v.ParentId,
                            DepartmentSK = v.DepartmentSK,
                            JobTransactionAmount = v.Inclusive ? v.TransactionAmount : v.TransactionAmount + v.TaxAmount,
                            TaxSK = v.TaxPercent == 0 && v.TaxPercent == 0 && v.TaxPercent == 0 ? v.TaxSK : null, // for 0 GST
                            TaxAmount = !string.IsNullOrWhiteSpace(v.TaxSK) && v.TaxPercent == 0 ? ConvertTaxAmount(sign: transactionAmount > 0 ? '+' : '-',
                            taxAmt: transactionAmount,
                            taxSK: v.TaxSK) : 0,

                            TaxPercent = 0, // for 0 GST
                            R1 = v.R1,
                            JobSK = v.JobSK,
                            RecordStatus = (int)RecordStatus.Post,
                            SourceHeaderTransSK = v.ParentId,
                            TransType = v.TransType,
                            SecondRef = v.R1,
                            ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                            CURRSK = v.CURRSK,
                            CURRATE = v.CURRATE,
                            CURROP = v.CURROP
                        });

                        if (v.TaxPercent > 0)
                        {
                            var taxAmount = v.TransType == (int)TransType.CashBookReceived ? decimal.Negate(v.TaxAmount) : v.TaxAmount;
                            bTableEntity.Add(new BTableEntity
                            {
                                AccSK = v.TransType == (int)TransType.CashBookReceived ? _tax.Finds(filter: f => f.SysKey == v.TaxSK).Single().OutputTaxSK
                                : _tax.Finds(filter: f => f.SysKey == v.TaxSK).Single().InputTaxSK,
                                PostDate = v.TransactionDate,
                                TransactionAmount = taxAmount,
                                IsTaxInclusive = v.Inclusive,
                                ParentId = v.ParentId,
                                RecordStatus = (int)RecordStatus.Post,
                                SourceHeaderTransSK = v.ParentId,
                                EntryDescription = cbt.Description,
                                LineDescription = v.Memo,
                                TaxAmount = ConvertTaxAmount(sign: taxAmount > 0 ? '+' : '-',
                                taxAmt: transactionAmount,
                                taxSK: v.TaxSK),

                                TaxPercent = v.TaxPercent,
                                R1 = v.R1,
                                TaxSK = v.TaxSK,
                                SecondRef = v.R1,
                                TransType = v.TransType,
                                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                                CURRSK = v.CURRSK,
                                CURRATE = v.CURRATE,
                                CURROP = v.CURROP
                            });
                        }
                    }

                    #endregion

                    #region CashBook Account

                    bTableEntity.Add(new BTableEntity
                    {
                        AccSK = _cashbookSetup.Finds(filter: f => f.SysKey == cbt.CBSK).Single().AccSK,
                        PostDate = cbt.TransactionDate,
                        SecondRef = cbt.R1,
                        ParentId = cbt.ParentId,
                        EntryDescription = cbt.Description,
                        LineDescription = cbt.Memo,
                        R1 = cbt.R1,
                        TransactionAmount = cbt.TransType == (int)TransType.CashBookReceived ?
                        entityList.Sum(x => x.TotalAmount) : decimal.Negate(entityList.Sum(x => x.TotalAmount)),
                        IsTaxInclusive = cbt.Inclusive,
                        SourceHeaderTransSK = cbt.ParentId,
                        TransType = cbt.TransType,
                        TaxAmount = 0,
                        TaxPercent = 0,
                        RecordStatus = (int)RecordStatus.Post,
                        ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                        CURRSK = cbt.CURRSK,
                        CURRATE = cbt.CURRATE,
                        CURROP = cbt.CURROP
                    });

                    #endregion

                    break;
                case TransType.CashBookReceived:

                    #region CashBook Account

                    bTableEntity.Add(new BTableEntity
                    {
                        AccSK = _cashbookSetup.Finds(filter: f => f.SysKey == cbt.CBSK).Single().AccSK,
                        PostDate = cbt.TransactionDate,
                        SecondRef = cbt.R1,
                        ParentId = cbt.ParentId,
                        EntryDescription = cbt.Description,
                        LineDescription = cbt.Memo,
                        R1 = cbt.R1,
                        TransactionAmount = cbt.TransType == (int)TransType.CashBookReceived ?
                        entityList.Sum(x => x.TotalAmount) : decimal.Negate(entityList.Sum(x => x.TotalAmount)),
                        IsTaxInclusive = cbt.Inclusive,
                        SourceHeaderTransSK = cbt.ParentId,
                        TransType = cbt.TransType,
                        TaxAmount = 0,
                        TaxPercent = 0,
                        RecordStatus = (int)RecordStatus.Post,
                        ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_RECEIPT,
                        CURRSK = cbt.CURRSK,
                        CURRATE = cbt.CURRATE,
                        CURROP = cbt.CURROP
                    });

                    #endregion

                    #region Prepare Account Detail

                    foreach (var v in entityList)
                    {
                        var transactionAmount = decimal.Negate(FindTransactionAmount(v.Inclusive, v.TaxAmount, v.TransactionAmount));
                        bTableEntity.Add(new BTableEntity
                        {
                            AccSK = v.AccSK,
                            PostDate = v.TransactionDate,
                            TransactionAmount = transactionAmount,
                            EntryDescription = cbt.Description,
                            LineDescription = v.Memo,
                            IsTaxInclusive = false,
                            ParentId = v.ParentId,
                            DepartmentSK = v.DepartmentSK,
                            JobTransactionAmount = decimal.Negate(v.Inclusive ? v.TransactionAmount : v.TransactionAmount + v.TaxAmount),
                            TaxSK = v.TaxPercent == 0 ? v.TaxSK : null, // for 0 GST
                            TaxAmount = v.TaxPercent == 0 && !string.IsNullOrWhiteSpace(v.TaxSK) ?
                                ConvertTaxAmount(sign: transactionAmount > 0 ? '+' : '-',
                                    taxAmt: transactionAmount,
                                    taxSK: v.TaxSK) : 0,
                            TaxPercent = 0, // for 0 GST
                            R1 = v.R1,
                            JobSK = v.JobSK,
                            RecordStatus = (int)RecordStatus.Post,
                            SourceHeaderTransSK = v.ParentId,
                            TransType = v.TransType,
                            SecondRef = v.R1,
                            ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_RECEIPT,
                            CURRSK = v.CURRSK,
                            CURRATE = v.CURRATE,
                            CURROP = v.CURROP
                        });

                        if (v.TaxPercent > 0)
                        {
                            var taxAmount = v.TransType == (int)TransType.CashBookReceived ? decimal.Negate(v.TaxAmount) : v.TaxAmount;
                            var accountSK = v.TransType == (int)TransType.CashBookReceived ?
                                    _tax.Finds(filter: f => f.SysKey == v.TaxSK).Single().OutputTaxSK
                                    : _tax.Finds(filter: f => f.SysKey == v.TaxSK).Single().InputTaxSK;
                            
                            bTableEntity.Add(new BTableEntity
                            {
                                AccSK = v.TransType == (int)TransType.CashBookReceived ?
                                    _tax.Finds(filter: f => f.SysKey == v.TaxSK).Single().OutputTaxSK
                                    : _tax.Finds(filter: f => f.SysKey == v.TaxSK).Single().InputTaxSK,
                                
                                PostDate = v.TransactionDate,
                                EntryDescription = cbt.Description,
                                LineDescription = v.Memo,
                                TransactionAmount = taxAmount,
                                IsTaxInclusive = v.Inclusive,
                                ParentId = v.ParentId,
                                RecordStatus = (int)RecordStatus.Post,
                                SourceHeaderTransSK = v.ParentId,
                                //TaxAmount = transactionAmount,

                                TaxAmount = ConvertTaxAmount(sign: taxAmount > 0 ? '+' : '-',
                                taxAmt: transactionAmount,
                                taxSK: v.TaxSK),

                                TaxPercent = v.TaxPercent,
                                R1 = v.R1,
                                TaxSK = v.TaxSK,
                                SecondRef = v.R1,
                                TransType = v.TransType,
                                ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_RECEIPT,
                                CURRSK = v.CURRSK,
                                CURRATE = v.CURRATE,
                                CURROP = v.CURROP
                            });
                        }
                    }

                    #endregion

                    break;
            }

            var diff = bTableEntity.Sum(x => x.TransactionAmount.ToRoundUp());
            if (diff != 0)
            {
                bTableEntity.Add(new BTableEntity
                {
                    AccSK = DI.Current.Get<EssAppServices>().DefaultAccountingCode.DecimalAdjustment.Key,
                    PostDate = cbt.TransactionDate,
                    TransactionAmount = decimal.Negate(diff),
                    IsTaxInclusive = false,
                    EntryDescription = cbt.Description,
                    LineDescription = cbt.Memo,
                    ParentId = cbt.ParentId,
                    DepartmentSK = cbt.DepartmentSK,
                    TaxPercent = 0, // for 0 GST
                    R1 = bTableEntity[0].R1,
                    RecordStatus = (int)RecordStatus.Post,
                    SourceHeaderTransSK = cbt.ParentId,
                    TransType = cbt.TransType,
                    SecondRef = cbt.R1,
                    ModuleCode = CommonConstant.Module.MODULE_CASHBOOK_PAYMENT,
                    CURRSK = cbt.CURRSK,
                    CURRATE = cbt.CURRATE,
                    CURROP = cbt.CURROP
                });
            }
            return bTableEntity;
        }


        #endregion

        #region Utilities

        private decimal FindTransactionAmount(
        bool inclusive,
        decimal taxAmount,
        decimal transactionAmount)
        {
            var compareAmount = 0.0M;

            if (inclusive)
            {
                compareAmount = transactionAmount - taxAmount;
            }
            else
            {
                compareAmount = transactionAmount;
            }
            return compareAmount;
        }


        #endregion
    }
}
