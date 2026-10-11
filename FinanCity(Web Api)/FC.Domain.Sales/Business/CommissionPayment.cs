namespace FC.Domain.Sales.Business
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Sales.Infrastructure;

    public class CommissionPayment : BusinessLogic<CommissionPaymentEntity, CommissionPaymentDAO>
    {
        #region Fields

        private CommissionAllocation _allocation;
        private SalesInvoice _salesInvoice;
        private PropertyTransactionDetail _salesDetail;
        private IBTableResult<CommissionPaymentEntity> _bTableResult;
        private CashbookSetup _cashbookSetup;
        private IEssAppRepository _app;
        private Contact _contact;
        private CashBookTransaction _cashbookTransaction;

        #endregion

        #region Ctor

        public CommissionPayment(CommissionAllocation allocation,
            CashBookTransaction cashbookTransaction,
            IEssAppRepository app,
            Contact contact,
            CashbookSetup cashbookSetup,
            PropertyTransactionDetail transactionDetail,
            CommissionPayableAccountResult bTableResult,
            SalesInvoice salesInvoice)
        {
            _cashbookTransaction = cashbookTransaction;
            _contact = contact;
            _app = app;
            _cashbookSetup = cashbookSetup;
            _bTableResult = bTableResult;
            _salesDetail = transactionDetail;
            _salesInvoice = salesInvoice;
            _allocation = allocation;
        }

        #endregion

        #region Methods

        public IDataPagerResult<CommissionPayableDTO> GetPager(int currentPage, int pageSize, Expression<Func<CommissionPayableDTO, bool>> where = null)
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

        public IDataPagerResult<CommissionPayableDetailDTO> GetDetailPager(int currentPage, int pageSize, Expression<Func<CommissionPayableDetailDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetDetailPager(currentPage: currentPage, pageSize: pageSize, where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void PostAsTransaction(CommissionPaymentEntity commissionPayment)
        {
            try
            {
                #region AMS

                commissionPayment.RecordStatus = (int)RecordStatus.Post;

                Update(entity: commissionPayment, filter: f => f.SysKey == commissionPayment.SysKey);
            
                var allocation = _allocation.Finds(filter: f => f.SysKey == commissionPayment.ALOSK).Single();

                if (CanSalesDetailClose(allocation.HeaderSK, allocation.DetailSK))
                {
                    _salesDetail.Update(selector: s => new PropertyTransactionDetailEntity
                    {
                        RecordStatus = (int)RecordStatus.Post, //required to put the record status
                        PayableStatus = (int)SalesViewType.Close
                    },
                    filter: f => f.SysKey == allocation.DetailSK);
                }

                if (CanSalesInvoiceClose(allocation.HeaderSK))
                {
                    _salesInvoice.Update(selector: s => new SalesInvoiceEntity
                    {
                        RecordStatus = (int)RecordStatus.Post, //required to put the record status
                        PayableStatus = (int)SalesViewType.Close
                    },
                    filter: f => f.SysKey == allocation.HeaderSK);
                }

                #endregion

                #region Accounting

                var result = _bTableResult.Convert(new List<CommissionPaymentEntity> { commissionPayment },
                    null);

                _bTableResult.OnJournalPosted += (sender, e) =>
                {
                    Update(selector: s => new CommissionPaymentEntity
                    {
                        JRef = e.JournalNo,
                        RecordStatus = (int)RecordStatus.Post //needs to assign explicite record status.
                    }, filter: f => f.SysKey == commissionPayment.SysKey);

                    #region CashBook

                    var allocate = _allocation.Finds(filter: f => f.SysKey == commissionPayment.ALOSK).First();
                    var invoice = _salesInvoice.Finds(filter: f => f.SysKey == allocate.HeaderSK).Single();
                    var contact = _contact.Finds(filter: f => f.SysKey == invoice.SysKey).Single();
                    var sod = _salesDetail.Finds(filter: f => f.SysKey == allocate.DetailSK).First();
                    var postingAccount = (sod.TransType == (int)TransType.InternalSalesperson || sod.TransType == (int)TransType.MainSalesperson) ?
                                _app.DefaultAccountingCode.SalespersonAccurals :
                                _app.DefaultAccountingCode.CobrokerAccurals;

                    _cashbookTransaction.PostAsTransaction(insertedItems: new List<CashBookTransactionEntity>
                    {
                        new CashBookTransactionEntity
                        {
                            CreatedDate = commissionPayment.DatePaid,
                            AccCode = postingAccount.Value,
                            AccSK = postingAccount.Key,
                            BankInfo = _cashbookSetup.Finds(filter: f => f.SysKey == commissionPayment.CBSK).Single().CBType == 2
                            ? _cashbookSetup.Finds(filter: f => f.SysKey == commissionPayment.CBSK).Single().Description : "",
                            CBSK = commissionPayment.CBSK,
                            ChequeNo = commissionPayment.ChequeNo,
                            JRef = e.JournalNo,
                            TransType = (int)TransType.CashBookPayment,
                            RefTransactionType = (int)TransType.CommissionPayable,
                            Description = $@"{TransType.CommissionPayable.ToDescription()}: {invoice.R1}, {commissionPayment.R1}",
                            Memo = invoice.R1,
                            CreatedTime = commissionPayment.CreatedTime,
                            Inclusive = false,
                            PaidPerson = "",
                            ParentId = GeneralUtility.Key,
                            SourceHeaderTransSK = commissionPayment.SysKey,
                            RecordStatus = (int)RecordStatus.Post,
                            Remark = "",
                            SecondRef = commissionPayment.R1,
                            TaxAmount = 0,
                            TaxPercent = 0,
                            TaxSK = "1",
                            TotalAmount = commissionPayment.NetPaid,
                            TransactionAmount = commissionPayment.NetPaid,
                            TransactionDate = commissionPayment.DatePaid,
                            CURRATE = commissionPayment.CURRATE,
                            CURROP = commissionPayment.CURROP,
                            CURRSK = commissionPayment.CURRSK
                        }
                    },
                    updatedItems: null,
                    deletedItems: null,
                    accounting : false);
                    #endregion
                };

                _bTableResult.ExcutePost(result.ToList());

                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ReverseAsTransaction(string key)
        {
            try
            {
                #region AMS

                Update(selector: (s) => new CommissionPaymentEntity
                {
                    RecordStatus = (int)RecordStatus.Update
                }, filter: f => f.SysKey == key);

                var payment = Finds(filter: (f) => f.SysKey == key).Single();
                var allocation = _allocation.Finds(filter: (f) => f.SysKey == payment.ALOSK).Single();

                _salesInvoice.Update(selector: (s) => new SalesInvoiceEntity
                {
                    RecordStatus = (int)RecordStatus.Post,
                    PayableStatus = (int)SalesViewType.Pending
                },
                filter: f => f.SysKey == allocation.HeaderSK);

                _salesDetail.Update(selector: (s) => new PropertyTransactionDetailEntity
                {
                    RecordStatus = (int)RecordStatus.Post,
                    PayableStatus = (int)SalesViewType.Pending
                }, filter: f => f.SysKey == allocation.DetailSK);

                #endregion

                #region Cashbook

                _cashbookTransaction.Update(selector: s => new CashBookTransactionEntity
                {
                    RecordStatus = (int)RecordStatus.Void
                }, filter: f => f.SourceHeaderTransSK == key);

                #endregion

                #region Accounting

                var entity = Finds(filter: f => f.SysKey == key).Single();

                _bTableResult.ExcuteReverse(key: key,
                    postDate: entity.DatePaid.Value,
                    module: CommonConstant.Module.MODULE_CASHBOOK_PAYMENT);

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void VoidAsTransaction(string key)
        {
            try
            {
                Update(selector: (s) => new CommissionPaymentEntity { RecordStatus = (int)RecordStatus.Void },
                   filter: f => f.SysKey == key);

                var entity = Finds(filter: f => f.SysKey == key).Single();

                _bTableResult.ExcuteVoid(key: key,
                    postDate: entity.DatePaid.Value,
                    module: CommonConstant.Module.MODULE_CASHBOOK_PAYMENT);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public DataTable CommissionPaymentPrinting(string key)
        {
            try
            {
                return DAO.CommissionPaymentPrinting(key);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private bool CanSalesDetailClose(string invoiceSK, string detailSK)
        {
            var detail = _salesDetail.Finds(filter: f => f.SysKey == detailSK).Single();
            var salespersonPayable = decimal.Zero;
            var salespersonPaid = decimal.Zero;
         
            _allocation.Finds(filter: f => f.HeaderSK == invoiceSK
             && f.DetailSK == detailSK).Each(a =>
             {
                 GetDetailPager(currentPage: 1,
                     pageSize: 14, where: f => f.ALOSK == a.SysKey).Data.Each(b =>
                     {
                         salespersonPaid += b.PaidAmount.ToRoundUp();
                         salespersonPayable += b.PayableAmount.ToRoundUp();
                     });
             });

            return (salespersonPayable - salespersonPaid == 0) && (detail.ReceivableStatus == (int)SalesViewType.Close);
        }

        private bool CanSalesInvoiceClose(string invoiceSK)
        {
            var invoice = _salesInvoice.Finds(filter: f => f.SysKey == invoiceSK).Single();
         
            var totalPaid = decimal.Zero;
            var totalPayable = decimal.Zero;

            GetPager(currentPage: 1,
                pageSize: 14, where: f => f.InvoiceSK == invoiceSK).Data.Each(a =>
                {
                    totalPaid += a.TotalPaid.ToRoundUp();
                    totalPayable += a.TotalPayable.ToRoundUp();
                });

            return totalPayable - totalPaid == 0 && invoice.ReceivableStatus == (int)SalesViewType.Close;
        }

        #endregion
    }
}
