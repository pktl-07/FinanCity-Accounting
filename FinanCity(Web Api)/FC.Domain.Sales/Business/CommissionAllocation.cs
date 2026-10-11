namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Core.Data;
    using Ninject;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Infrastructure;
    using FC.Domain.Accounts;
   
    public class CommissionAllocation : BusinessLogic<AllocationEntity, AllocationDAO>
    {
        #region Fields

        private IEssAppRepository _app;
        private CommissionStructureDetail _commissionStructure
        {
            get { return DI.Current.Get<CommissionStructureDetail>(); }
        }
        private CommissionHelper _commissionHelper
        {
            get { return DI.Current.Get<CommissionHelper>(); }
        }
        private SalesInvoice _saleInvoice
        {
            get { return DI.Current.Get<SalesInvoice>(); }
        }
        public OverridingProfile _overrideProfile
        {
            get { return DI.Current.Get<OverridingProfile>(); }
        }
        private PropertyTransactionDetail _saleOrderDetail
        {
            get { return DI.Current.Get<PropertyTransactionDetail>(); }
        }
        private CommissionReceived _commissionReceived
        {
            get { return DI.Current.Get<CommissionReceived>(); }
        }
        private PropertyTransaction _propertyTransaction
        {
            get { return DI.Current.Get<PropertyTransaction>(); }
        }
        private CommissionPayment _commissionPayment
        {
            get { return DI.Current.Get<CommissionPayment>(); }
        }
        private CommissionHelperFactory _commissionHelperFactory
        {
            get { return DI.Current.Get<CommissionHelperFactory>(); }
        }
        private Person _person
        {
            get { return DI.Current.Get<Person>(); }
        }
        private PayableScheme _payableScheme
        {
            get { return DI.Current.Get<PayableScheme>(); }
        }
        private Employment _employemnt
        {
            get { return DI.Current.Get<Employment>(); }
        }
        private BonusPayable _bonusPayable
        {
            get { return DI.Current.Get<BonusPayable>(); }
        }

        private IBTableResult<AllocationEntity> _bTableResult;

        #endregion

        #region Ctor

        public CommissionAllocation(CommissionAllocationAccountResult bTableResult,
            IEssAppRepository app)
        {
            _app = app;
            _bTableResult = bTableResult;

            base.CanReverseOrVoid = (syskey) =>
            {
                _payableScheme.Finds(filter: f => f.ALOSK == syskey)
                .GroupBy(x => new { x.SPSK, x.AssociateComm })
                .Select(group => new { count = group.Count(), group.Key.SPSK })
                .Where(x => x.count > 1).Each(x =>
                {
                    throw new Exception("Some of the salesperson has a promotion case.");
                });

                _commissionPayment.Finds(filter: f => f.ALOSK == syskey &&
                f.RecordStatus == (int)RecordStatus.Post).Each(pay =>
                {
                    throw new Exception($@"The regarding Payment No. {pay.R1} should be void.");
                });

                return true;
            };
        }

        #endregion

        #region Methods

        public IDataPagerResult<CommissionReceivedAllocationDTO> GetPager(int currentPage, int pageSize, Expression<Func<CommissionReceivedAllocationDTO, bool>> where = null)
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

        public decimal GetSalesCommissionToDate(string spsk)
        {
            try
            {
                return DAO.GetSalesCommissoinToDate(spsk: spsk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetSalesCommissionToYear(string spsk, int fromDate, int toDate)
        {
            try
            {
                return DAO.GetSalesCommissionToYear(spsk: spsk, fromDate: fromDate, toDate: toDate);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetCloseSalesValueToDate(string spsk)
        {
            try
            {
                return DAO.GetSalesValueToDate(spsk: spsk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal GetCloseSalesValueToYear(string spsk, int fromDate, int toDate)
        {
            try
            {
                return DAO.GetSalesValueToYear(spsk: spsk, fromDate: fromDate, toDate: toDate);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void PostAsTransaction(AllocationEntity entity, 
            bool autoAllocate = true)
        {
            try
            {
                Func<PersonEntity, bool> IsInternalSalesperson = (sp) =>
                {
                    return sp.TransType == (int)TransType.InternalSalesperson;
                };

                entity.GrossToPromote = CanGrossToPromote(entity.SPSK, entity.HeaderSK);
                entity.MarketingExpense = BindMarketingExpense(
                    detailsk: entity.DetailSK,
                    totalAllocateAmount: entity.AmountBeforeGST);

                var salesPerson = _person.Finds(filter: f => f.SysKey == entity.SPSK).Single();
                var saleOrder = _propertyTransaction.Finds(filter: f => f.SysKey == entity.HeaderSK).Single();
                var nextLevelPromotion = _commissionHelper.FindGrossToNextPromote(entity.SPSK);

                if (autoAllocate)
                {
                    entity = Insert(entity);
                }
                switch (entity.TransType)
                {
                    //case (int)TransType.CreditNote:
                    //case (int)TransType.DebitNote:
                    case (int)TransType.AllocationCommission:

                        entity.RecordStatus = (int)RecordStatus.Post;
                        Update(entity: entity, filter: f => f.SysKey == entity.SysKey);

                        var promotionResult = _commissionHelperFactory.GetContext(
                        invoiceSK: entity.HeaderSK,
                        saleOrderDetailSK: entity.DetailSK,
                        spsk: entity.SPSK,
                        allocatedAmt: IsInternalSalesperson(salesPerson) ? (entity.AmountBeforeGST.ToRoundUp() - entity.MarketingExpense.ToRoundUp())
                        : entity.AllocateAmount.ToRoundUp() - entity.MarketingExpense.ToRoundUp(),
                         alosk: entity.SysKey,
                         batchNo: saleOrder.ORBatchNo,
                         grossToPromoteNext: nextLevelPromotion);

                        switch (promotionResult.SalespersonEntity.TransType)
                        {
                            case (int)TransType.InternalSalesperson:

                                _employemnt.Update(promotionResult.EmploymentEntity, filter: f => f.SysKey == promotionResult.EmploymentEntity.SysKey);
                                _payableScheme.BulkInsert(promotionResult.AMS104List);
                                _bonusPayable.BulkInsert(promotionResult.AMS105List ?? new List<BonusPayableEntity>());

                                var allocation = Finds(filter: f => f.SysKey == entity.SysKey);
                                var bTableResult = _bTableResult.Convert(entityList: allocation, parameters: null);

                                if (_app.EnableSalespersonTierBonus)
                                {
                                    _bTableResult.OnJournalPosted += (sender, e) =>
                                    {
                                        Update(selector: s => new AllocationEntity
                                        {
                                            RecordStatus = (int)RecordStatus.Post,
                                            JRef = e.JournalNo,
                                        }, filter: f => f.SysKey == entity.SysKey);
                                    };
                                    _bTableResult.ExcutePost(bTableResult);
                                }

                                break;
                            case (int)TransType.ExternalSalesPerson:

                                _payableScheme.BulkInsert(promotionResult.AMS104List);

                                break;
                        }
                        break;
                }

                UpdateSaleStatus(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ReverseAsTransaction(string syskey)
        {
            if (this.CanReverseOrVoid(syskey))
            {
                var allocation = Finds(filter: f => f.SysKey == syskey).Single();

                _payableScheme.Finds(filter: f => f.ALOSK == syskey).Each(p =>
                {
                    _bonusPayable.Delete(filter: f => f.PayableSchemeSK == p.SysKey);
                    _payableScheme.Delete(filter: f => f.SysKey == p.SysKey);
                });

                _saleInvoice.Update(selector: s => new SalesInvoiceEntity
                {
                    RecordStatus = (int)RecordStatus.Post,
                    ReceivableStatus = (int)FC.Core.SalesViewType.Pending
                },
                filter: f => f.SysKey == allocation.HeaderSK);

                _commissionPayment.Update(selector: s => new CommissionPaymentEntity
                {
                    RecordStatus = (int)RecordStatus.Void,
                }, filter: f => f.ALOSK == allocation.SysKey);

                _saleOrderDetail.Update(selector: s => new PropertyTransactionDetailEntity
                {
                    RecordStatus = (int)RecordStatus.Post,
                    ReceivableStatus = (int)FC.Core.SalesViewType.Pending
                },
                filter: f => f.SysKey == allocation.DetailSK);

                this.Update(selector: (s) => new AllocationEntity { RecordStatus = (int)RecordStatus.Update },
                    filter: f => f.SysKey == syskey);

                if (_app.EnableSalespersonTierBonus)
                {
                    var entity = Finds(filter: f => f.SysKey == syskey).Single();

                    _bTableResult.ExcuteReverse(key: entity.SysKey,
                        postDate: entity.AllocationDate.Value,
                        module: CommonConstant.Module.MODULE_SALE); ;
                }
            }
        }

        public void VoidAsTransaction(string syskey)
        {
            if (CanReverseOrVoid(syskey))
            {
                Update(selector: (s) => new AllocationEntity
                {
                    RecordStatus = (int)RecordStatus.Void
                },
                filter: f => f.SysKey == syskey);

                if (_app.EnableSalespersonTierBonus)
                {
                    var entity = Finds(filter: f => f.SysKey == syskey).Single();

                    _bTableResult.ExcuteVoid(key: entity.SysKey,
                         postDate: entity.AllocationDate.Value,
                         module: CommonConstant.Module.MODULE_SALE);
                }
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

        public decimal SalespersonTotalReceivable(string spsk, string invoicesk)
        {
            try
            {
                var detail = _saleOrderDetail.Finds(x => x.PTOSK == invoicesk
                && x.SPSK == spsk
                && x.RecordStatus == (int)RecordStatus.Post)
                  .SingleOrDefault();

                if (detail != null)
                {

                    //var credit = detail.Credit + ((detail.Credit / 100) * detail.TaxPercent);
                    //var debit = detail.Debit + ((detail.Debit / 100) * detail.TaxPercent);

                    return (detail.GrossComm.ToRoundUp());
                }

                return decimal.Zero;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public decimal SalespersonTotalReceived(string spsk, string invoicesk)
        {
            try
            {
                var totalReceived = Finds(x => x.HeaderSK == invoicesk && x.SPSK == spsk && x.RecordStatus == (int)RecordStatus.Post &&
                x.TransType == (int)TransType.AllocationCommission)
                    .Sum(x => x.AllocateAmount.ToRoundUp());

                return totalReceived;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void UpdateSaleStatus(AllocationEntity entity)
        {
            var invoice = _saleInvoice.Finds(filter: f => f.SysKey == entity.HeaderSK).Single();
            var recivedAmt = _commissionReceived.Finds(filter: f => f.InvoiceSK == entity.HeaderSK && f.RecordStatus == (int)RecordStatus.Post).Sum(x => x.TotalAmt.ToRoundUp());

            var d = _saleOrderDetail.Finds(filter: f => f.SysKey == entity.DetailSK).Single();
            if (d.GrossComm.ToRoundUp() == entity.AmountBeforeGST.ToRoundUp())
            {
                d.ReceivableStatus = (int)SalesViewType.Close;
                _saleOrderDetail.Update(d, filter: f => f.SysKey == d.SysKey);
            }

            if (invoice.ActualAmt.ToRoundUp() - recivedAmt.ToRoundUp() <= 0)
            {
                invoice.ReceivableStatus = (int)SalesViewType.Close;
                _saleInvoice.Update(invoice, filter: f => f.SysKey == invoice.SysKey);

                // update all sales details
                foreach (var detail in _saleOrderDetail.Finds(filter: f => f.PTOSK == invoice.SysKey))
                {
                    detail.ReceivableStatus = (int)SalesViewType.Close;
                    _saleOrderDetail.Update(detail, filter: f => f.SysKey == detail.SysKey);
                }
            }
        }

        private bool CanGrossToPromote(string spsk, string invoicesk)
        {
            var sod = _saleOrderDetail.Finds(filter: f => f.SPSK == spsk && f.PTOSK == invoicesk).Single();
            var transaction = _propertyTransaction.Finds(filter: f => f.SysKey == invoicesk).Single();
            var scheme = _commissionStructure.Finds(filter: f => f.SysKey == sod.SchemeDetailSK).Single();

            return !(scheme.Type == (int)FC.Core.CommissonStructureType.CompanyLeads ||
            scheme.Type == (int)FC.Core.CommissonStructureType.PersonalLeads ||
            scheme.Type == (int)FC.Core.CommissonStructureType.RESLeads);
        }

        private decimal BindMarketingExpense(
            string detailsk,
            decimal totalAllocateAmount)
        {
            var salesDetail = _saleOrderDetail.Finds(filter: f => f.SysKey == detailsk).Single();
            var receivable = salesDetail.GrossComm;
            var marketingExpense = salesDetail.MarketingExpense;
           
            return ((totalAllocateAmount.ToRoundUp() / receivable.ToRoundUp()) * marketingExpense.ToRoundUp());
        }

        #endregion
    }
}
