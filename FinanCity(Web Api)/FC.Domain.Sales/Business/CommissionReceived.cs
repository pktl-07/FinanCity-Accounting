namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Sales.Infrastructure;

    public class CommissionReceived : BusinessLogic<CommissionReceivedEntity, CommissionReceivedDAO>
    {
        #region Fields

        private PropertyTransactionDetail _propertyTransactionDetail;
        private CommissionAllocation _commissionAllocation;
        private IBTableResult<CommissionReceivedEntity> _bTableResult;
        private CashbookSetup _cashbookSetup;
        private CashBookTransaction _cashbookTransaction;
        private IEssAppRepository _app;
        private SalesInvoice _salesInvoice;

        #endregion

        #region Ctor

        public CommissionReceived(
            SalesInvoice invoice,
            CashbookSetup cashbookSetup,
            IEssAppRepository app,
            CashBookTransaction cashbookTransaction,
            CommissionReceivableAccountResult bTableResult,
            PropertyTransactionDetail propertyTransactionDetail,
            CommissionAllocation commissionAllocation)
        {
            _salesInvoice = invoice;
            _cashbookSetup = cashbookSetup;
            _app = app;
            _cashbookTransaction = cashbookTransaction;
            _propertyTransactionDetail = propertyTransactionDetail;
            _commissionAllocation = commissionAllocation;
            _bTableResult = bTableResult;

            CanReverseOrVoid = (syskey) =>
            {
                var invoiceSK = Finds(filter: f => f.SysKey == syskey).Single().InvoiceSK;
                _commissionAllocation.Finds(
                    filter: f => f.HeaderSK == invoiceSK && f.RecordStatus == (int)RecordStatus.Post).Each(alo =>
                {
                    throw new Exception($@"The regarding Allocation '{alo.R1}' should be void.");
                });
                return true;
            };
        }

        #endregion

        #region Methods

        public IDataPagerResult<CommissionReceivedDTO> GetPager(int currentPage, int pageSize, Expression<Func<CommissionReceivedDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage: currentPage, pageSize: pageSize, where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<CommissionReceivedDTO> GetCommissionReceivedDTO(Expression<Func<CommissionReceivedDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetCommissionReceivedDTO(where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void PostAsTransaction(CommissionReceivedEntity received)
        {
            try
            {
                received.RecordStatus = (int)RecordStatus.Post;
                Update(entity: received, filter: f => f.SysKey == received.SysKey);

                bool autoAllocation = false;

                foreach (var alo in GetAllocationList(received, ref autoAllocation))
                    _commissionAllocation.PostAsTransaction(alo, autoAllocation);

                var result = _bTableResult.Convert(new List<CommissionReceivedEntity> { received },
                    null);

                _bTableResult.OnJournalPosted += (sender, e) =>
                {
                    received.JRef = e.JournalNo;

                    Update(selector: s => new CommissionReceivedEntity
                    {
                        RecordStatus = (int)RecordStatus.Post,
                        JRef = e.JournalNo,
                    }, filter: f => f.SysKey == received.SysKey);

                    #region CashBook
                 
                    var salesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == received.InvoiceSK).Single();
                    _cashbookTransaction.PostAsTransaction(insertedItems: new List<CashBookTransactionEntity>
                    {  new CashBookTransactionEntity
                        {
                            CreatedDate = received.ReceiptDate,
                            AccCode = _app.DefaultAccountingCode.CommisionReceivable.Key.Replace("-", ""),
                            AccSK = _app.DefaultAccountingCode.CommisionReceivable.Key,
                            BankInfo = _cashbookSetup.Finds(filter: f => f.SysKey == received.CBSK).Single().CBType == (int)CashBookType.Bank ? _cashbookSetup.Finds(filter: f => f.SysKey == received.CBSK).Single().Description : "",
                            CBSK = received.CBSK,
                            JRef = e.JournalNo,
                            ChequeNo = received.ChequeNo,
                            TransType = (int)TransType.CashBookReceived,
                            Description = $"{TransType.CommissionReceivable.ToDescription()}: {salesInvoice.R1}, {received.R1}",
                            Memo = salesInvoice.R1,
                            CreatedTime = salesInvoice.CreatedTime,
                            Inclusive = false,
                            ParentId = GeneralUtility.Key,
                            SourceHeaderTransSK = received.SysKey,
                            RecordStatus = (int)RecordStatus.Post,
                            Remark = "",
                            SecondRef = received.R1,
                            TaxAmount = 0,
                            TaxPercent = 0,
                            TaxSK = "1",
                            TotalAmount = received.TotalAmt,
                            TransactionAmount = received.TotalAmt,
                            TransactionDate = received.ReceiptDate,
                            RefTransactionType = (int)TransType.CommissionReceivable,
                            CURRSK = received.CURRSK,
                            CURRATE = received.CURRATE,
                            CURROP = received.CURROP
                        }
                    },
                    updatedItems: null,
                    deletedItems: null,
                    accounting: false);
                
                    #endregion
                };

                _bTableResult.ExcutePost(result.ToList());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ReverseAsTransaction(string sysKey)
        {
            try
            {
                var invoiceSK = Finds(filter: f => f.SysKey == sysKey).Single().InvoiceSK;
                if (CanReverseOrVoid(sysKey))
                {
                    Update(selector: (s) => new CommissionReceivedEntity { RecordStatus = (int)RecordStatus.Update },
                        filter: f => f.SysKey == sysKey);

                    var entity = Finds(filter: f => f.SysKey == sysKey).Single();

                    _commissionAllocation.Update(selector: s => new AllocationEntity
                    {
                        RecordStatus = (int)RecordStatus.Void
                    }, filter: f => f.HeaderSK == invoiceSK);

                    _cashbookTransaction.Update(selector: s => new CashBookTransactionEntity
                    {
                        RecordStatus = (int)RecordStatus.Void
                    }, filter: f => f.SourceHeaderTransSK == sysKey);

                    _bTableResult.ExcuteReverse(key: sysKey,
                        postDate: entity.ReceiptDate.Value,
                        module: CommonConstant.Module.MODULE_CASHBOOK_RECEIPT);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void VoidAsTransaction(string sysKey)
        {
            if (CanReverseOrVoid(sysKey))
                Update(selector: (s) => new CommissionReceivedEntity { RecordStatus = (int)RecordStatus.Void }, filter: f => f.SysKey == sysKey);

            var entity = Finds(filter: f => f.SysKey == sysKey).Single();

            _bTableResult.ExcuteVoid(key: sysKey,
                    postDate: entity.ReceiptDate.Value,
                    module: CommonConstant.Module.MODULE_CASHBOOK_RECEIPT);
        }

        public IEnumerable<BTableEntity> RecapAsTransaction(string key)
        {
            try
            {
                var p = Finds(filter: f => f.SysKey == key);

                return _bTableResult.Convert(entityList: p, parameters: null);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable CommissionReceivedTemplateDataTable(string key)
        {
            try
            {
                return DAO.CommissionReceivedPrinting(key);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private List<AllocationEntity> GetAllocationList(CommissionReceivedEntity received,
        ref bool allocationAuto)
        {
            var detail = _propertyTransactionDetail.Finds(filter: f => f.PTOSK == received.InvoiceSK);
            var balance = GetCommissionReceivedDTO(where: f => f.InvoiceSK == received.InvoiceSK).First().Balance.ToRoundUp();
            var allocationList = new List<AllocationEntity>();

            Func<ReceivableStatus> status = () =>
            {
                if (balance == 0)
                    return ReceivableStatus.FullReceived;

                if (detail.Count() == 1)
                    return ReceivableStatus.PartialWithOneSalespreson;

                return ReceivableStatus.PartialWithMultiSalesperson;
            };

            foreach (var d in detail)
            {
                var allocation = _commissionAllocation.Finds(filter: f => f.DetailSK == d.SysKey && f.RecordStatus == (int)RecordStatus.Post);
                var allocationGross = allocation.Where(x => x.TransType == (int)TransType.AllocationCommission)
                    .Sum(x => x.AmountBeforeGST.ToRoundUp());

                var allocationBalance = d.GrossComm - allocationGross;

                if (allocationBalance > 0)
                {
                    var new_AllocateAmount = 0M;
                    var new_AmountBeforeGST = 0M;
                    var new_GSTValue = 0.0M;
                    var new_RecordStatus = 0;
                    var new_TransType = 0;

                    switch (status())
                    {
                        case ReceivableStatus.FullReceived:

                            new_RecordStatus = (int)RecordStatus.Post;
                            new_AllocateAmount = allocationBalance + (allocationBalance / 100) * d.TaxPercent;
                            new_GSTValue = received.GSTValue / detail.Count();
                            new_AmountBeforeGST = allocationBalance;
                            new_TransType = (int)TransType.AllocationCommission;
                            allocationAuto = true;

                            allocationList.Add(new AllocationEntity
                            {
                                RecordStatus = new_RecordStatus,
                                AllocateAmount = new_AllocateAmount,
                                AllocationDate = received.ReceiptDate,
                                GSTPercent = d.TaxPercent,
                                AmountBeforeGST = new_AmountBeforeGST,
                                GSTValue = new_GSTValue,
                                HeaderSK = received.InvoiceSK,
                                DetailSK = d.SysKey,
                                TransType = new_TransType,
                                SPSK = d.SPSK,
                            });

                            break;

                        case ReceivableStatus.PartialWithMultiSalesperson:

                            new_RecordStatus = (int)RecordStatus.New;
                            new_AllocateAmount = 0;
                            new_GSTValue = 0;
                            new_AmountBeforeGST = 0;
                            new_TransType = (int)TransType.AllocationCommission;
                            allocationAuto = false;

                            break;

                        case ReceivableStatus.PartialWithOneSalespreson:

                            new_RecordStatus = (int)RecordStatus.Post;
                            new_AmountBeforeGST = received.AmountBeforeGST;
                            new_AllocateAmount = received.TotalAmt;
                            new_TransType = (int)TransType.AllocationCommission;
                            allocationAuto = true;

                            allocationList.Add(new AllocationEntity
                            {
                                RecordStatus = new_RecordStatus,
                                AllocateAmount = new_AllocateAmount,
                                AllocationDate = received.ReceiptDate,
                                GSTPercent = d.TaxPercent,
                                AmountBeforeGST = new_AmountBeforeGST,
                                GSTValue = new_GSTValue,
                                HeaderSK = received.InvoiceSK,
                                DetailSK = d.SysKey,
                                TransType = new_TransType,
                                SPSK = d.SPSK,
                            });

                            break;
                    }
                }
            }
            return allocationList;
        }

        private enum ReceivableStatus
        {
            FullReceived,
            PartialWithOneSalespreson,
            PartialWithMultiSalesperson
        }

        #endregion
    }
}
