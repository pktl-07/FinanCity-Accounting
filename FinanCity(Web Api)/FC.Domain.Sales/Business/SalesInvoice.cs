namespace FC.Domain.Sales.Business
{
    using System;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using Ninject;
    using FC.Domain.Accounts;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Sales.Infrastructure;
    using FC.Core.Exceptions;

    public class SalesInvoice : BusinessLogic<SalesInvoiceEntity, SalesInvoiceDAO>
    {
        #region Fields

        private OverridingScheme _overridingScheme = DI.Current.Get<OverridingScheme>();
        private OverridingProfile _overridingProfile = DI.Current.Get<OverridingProfile>();
        private SalesInvoiceLog _salesInvoiceLog;
        private IDIndexing _indexing;

        private Contact _contact
        {
            get { return DI.Current.Get<Contact>(); }
        }

        private PropertyTransactionDetail _propertyTransactionDetail
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

        private IBTableResult<SalesInvoiceEntity> _bTableResult;

        #endregion

        #region Ctor

        public SalesInvoice(SalesInvoiceAccountResult bTableResult,
            IDIndexing indexing,
            SalesInvoiceLog salesInvoiceLog)
        {
            _indexing = indexing;
            _bTableResult = bTableResult;
            _salesInvoiceLog = salesInvoiceLog;
            
            CanReverseOrVoid = (key) =>
            {
                _commissionReceived.Finds(filter: f => f.InvoiceSK == key && f.RecordStatus == (int)RecordStatus.Post).Each(rec =>
                {
                    throw new Exception($@"The regarding Received {rec.R1} should be void.");
                });

                return true;
            };
        }

        #endregion

        #region Methods

        public IDataPagerResult<InvoiceDTO> GetPager(int currentPage, int pageSize, Expression<Func<InvoiceDTO, bool>> filter = null)
        {
            try
            {
                return DAO.GetPager(currentPage: currentPage, pageSize: pageSize, filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateAsTransaction(SalesInvoiceEntity salesInvoice,
            ContactEntity contact,
            RecordStatus recordStatus = RecordStatus.Update)
        {
            try
            {
                //it needs to explicit set the record status because of the framework dependency check the CustomizeUpdateBuilder at ESS.WEb.Common assembly.
                salesInvoice.RecordStatus = (int)recordStatus;

                Update(salesInvoice, filter: f => f.SysKey == salesInvoice.SysKey);
                _contact.Update(contact, filter: f => f.SysKey == salesInvoice.SysKey);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void PostAsTransaction(SalesInvoiceEntity salesInvoice,
            ContactEntity contact)
        {
            try
            {
                UpdateAsTransaction(salesInvoice: salesInvoice,
                    contact: contact,
                    recordStatus: RecordStatus.Post);

                var btableEntityList = _bTableResult.Convert(new List<SalesInvoiceEntity> { salesInvoice }, null);

                _bTableResult.OnJournalPosted += (sender, e) =>
                {
                    Update(selector: a => new SalesInvoiceEntity
                    {
                        RecordStatus = (int)RecordStatus.Post,
                        JRef = e.JournalNo
                    }, filter: f => f.SysKey == salesInvoice.SysKey);
                };

                _bTableResult.ExcutePost(btableEntityList);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void VoidAsTransaction(string syskey)
        {
            try
            {
                if (CanReverseOrVoid(syskey))
                    Update(selector: (s) => new SalesInvoiceEntity { RecordStatus = (int)RecordStatus.Void },
                        filter: f => f.SysKey == syskey);

                var invoice = Finds(filter: f => f.SysKey == syskey).Single();

                _bTableResult.ExcuteVoid(key: syskey,
                      postDate: invoice.InvoiceDate.Value,
                      module: CommonConstant.Module.MODULE_PURCHASE);

                _bTableResult.ExcuteVoid(key: syskey,
                      postDate: invoice.InvoiceDate.Value,
                      module: CommonConstant.Module.MODULE_SALE);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ReverseAsTransaction(string syskey)
        {
            try
            {
                if (CanReverseOrVoid(syskey))
                    Update(selector: (s) => new SalesInvoiceEntity { RecordStatus = (int)RecordStatus.Update },
                        filter: f => f.SysKey == syskey);

                var invoice = Finds(filter: f => f.SysKey == syskey).Single();

                _commissionReceived.Finds(filter: f => f.InvoiceSK == syskey).Each(rec =>
                {
                    _commissionReceived.VoidAsTransaction(sysKey: rec.SysKey);
                });

                _bTableResult.ExcuteReverse(key: syskey,
                       postDate: invoice.InvoiceDate.Value,
                       module: CommonConstant.Module.MODULE_SALE);

                _bTableResult.ExcuteReverse(key: syskey,
                       postDate: invoice.InvoiceDate.Value,
                       module: CommonConstant.Module.MODULE_PURCHASE);
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

        /// <summary>
        /// Get Total Bonus Percentage for perticular Transaction for Team Manager Bonus Depends on Invoice
        /// </summary>
        /// <param name="sysKey">Invoice SK</param>
        /// <param name="SPSK">Salesperson SK</param>
        /// <returns>Total Percentage</returns>
        public decimal Get_TM_BonusPercent(string sysKey, string SPSK)
        {
            return GetORBonusPercent(sysKey, SPSK,
                _propertyTransactionDetail.Finds(filter: f => f.PTOSK == sysKey
                 && f.SPSK == SPSK).Single().SchemeDetailSK);
        }

        /// <summary>
        /// Get Total Bonus Percentage for perticular Transaction for Team Manager Bonus by finding manual
        /// </summary>
        /// <param name="PTOSK">Invoice SK</param>
        /// <param name="SPSK">Salesperson SK</param>
        /// <param name="schemeDetailSK">Scheme Details SK</param>
        /// <returns>Total Percentage</returns>
        public decimal GetORBonusPercent(string PTOSK, string SPSK, string schemeDetailSK)
        {
            var result = decimal.Zero;
            var saleOrder = _propertyTransaction.Finds(filter: f => f.SysKey == PTOSK).Single();
            var overrideSchemeList = _overridingScheme.Finds(filter: f => f.BatchNo == saleOrder.ORBatchNo);
            var profileList = _overridingProfile.Finds(filter: f => f.PTOSK == PTOSK && f.SPSK == SPSK);
            var overrideProfile = profileList.Last();
            var overrideScheme = overrideSchemeList.Where(x => x.SchemeSK == schemeDetailSK).Single();
            var tireRankList = _overridingProfile.GetTireRank(overrideProfile, overrideScheme);

            foreach (var tireRank in tireRankList)
            {
                if (!string.IsNullOrEmpty(tireRank.TireSK))
                {
                    if (!tireRank.TireSK.Equals(SalepersonStatus.Dummy.ToDescription(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        result += tireRank.Percentage;
                    }
                }
            }

            return result;
        }

        public DataTable InvoiceTemplateDataTable(string invoicesk)
        {
            try
            {
                return DAO.InvoiceTemplateDataTable(invoicesk: invoicesk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetSingleKey(string key)
        {
            try
            {
                var log = _salesInvoiceLog.Finds(filter: f => f.SysKey == key).SingleOrDefault();
                if (log != null)
                    return log.PTOSK;

                return key;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public InvoiceIndexingContext GetCustomIndexingContext()
        {
            try
            {
                var indexing = _indexing.Finds(filter: f => f.TransType == (int)TransType.Invoice).Single();
                var lastid = DAO.LastVoucherId(prefix: indexing.Prefix,
                    postfix: indexing.Postfix) + 1;

                if (lastid > indexing.EndValue)
                    throw new VoucherNumberExceedException();

                var template = GetZeroTemplate(indexing.EndValue);
                var r1 = lastid.ToString(template);

                return new InvoiceIndexingContext
                {
                    R1 = $"{indexing.Prefix}{r1}{indexing.Postfix}",
                    Prefix = indexing.Prefix,
                    Postfix = indexing.Postfix,
                    VoucherId = lastid,
                };
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string GetZeroTemplate(int num)
        {
            var length = num.ToString().Length;
            var result = "";
            for (int i = 0; i < length; i++)
                result += "0";

            return result;
        }

        public class InvoiceIndexingContext
        {
            public string R1 { get; set; }
            public string Prefix { get; set; }
            public string Postfix { get; set; }
            public int VoucherId { get; set; }
        }

        #endregion
    }
}
