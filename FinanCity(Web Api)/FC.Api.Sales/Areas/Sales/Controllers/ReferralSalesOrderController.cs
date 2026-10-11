namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using Ninject;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
  
    [RoutePrefix("ReferralSalesOrder")]
    public class ReferralSalesOrderController : BaseApiController
    {
        #region Fields

        private TNC _tnc;
        private KendoGridFilterTransalator _kendoGridFilter;
        private PropertyTransaction _propertyTransaction;
        private PropertyTransactionDetail _propertyTransactionDetail;
        private Contact _contact;
        private PTOClient _ptoClient;
        private ReferralSalesOrderValidator _validator;
        private Company _myCompany;
        private Tax _tax;
        private MarketingExpense _promotionItemExpense;
        private Employment _employment;
       
        #endregion

        #region Ctor

        public ReferralSalesOrderController(
            Employment employment,
            MarketingExpense promotionItemExpense,
            Company myCompany,
            Tax tax,
            TNC tnc,
            PropertyTransaction propertyTransaction,
            ReferralSalesOrderValidator validator,
            PropertyTransactionDetail propertyTransactionDetail,
            Contact contact,
            KendoGridDataCollector<PropertyTransactionDetailEntity> internalGridDataCollector,
            PTOClient ptoClient,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _employment = employment;
            _promotionItemExpense = promotionItemExpense;
            _tax = tax;
            _myCompany = myCompany;
            _validator = validator;
            _tnc = tnc;
            _propertyTransactionDetail = propertyTransactionDetail;
            _ptoClient = ptoClient;
            _contact = contact;
            _propertyTransaction = propertyTransaction;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<PropertyTransactionDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PropertyTransactionDTO>(h);
                e = e.Constant((x) => x.TransType == (int)TransType.ReferralSalesOrder);

                return _propertyTransaction.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<PropertyTransactionEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PropertyTransactionEntity>(h);
                e = e.Constant((x) => x.TransType == (int)TransType.ReferralSalesOrder);

                return _propertyTransaction.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public ReferralSalesOrderModel Find(string key)
        {
            try
            {
                return this.GetDefaultModel(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public ReferralSalesOrderModel Default()
        {
            try
            {
                return GetDefaultModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(ReferralSalesOrderModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.PropertyTransaction = _propertyTransaction.InsertAsTransaction
                        (property: model.PropertyTransaction,
                        content: model.Contact,
                        tnc: model.TNC,
                        clients: ConvertToClient(model),
                        internals: ConvertToSalesperson(model),
                        externals: new List<PropertyTransactionDetailEntity>(),
                        insertedExpense: new List<MarketingExpenseEntity>());

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.PropertyTransaction.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(ReferralSalesOrderModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _propertyTransaction.UpdateAsTransaction(property: model.PropertyTransaction,
                         contact: model.Contact,
                         tnc: model.TNC,
                         insertedClients: new List<PTOClientEntity>(),
                         updatedClients: ConvertToClient(model: model),
                         deletedClients: new List<PTOClientEntity>(),

                         insertedInternals: new List<PropertyTransactionDetailEntity>(),
                         updatedInternals: ConvertToSalesperson(model: model),
                         deletedInternals: new List<PropertyTransactionDetailEntity>(),

                         insertedExternals: new List<PropertyTransactionDetailEntity>(),
                         updatedExternals: new List<PropertyTransactionDetailEntity>(),
                         deletedExternals: new List<PropertyTransactionDetailEntity>(),

                         insertedExpense: new List<MarketingExpenseEntity>(),
                         updatedExpense: new List<MarketingExpenseEntity>(),
                         deletedExpense: new List<MarketingExpenseEntity>());

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.PropertyTransaction.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("post")]
        public IHttpActionResult Post(ReferralSalesOrderModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _propertyTransaction.PostAsTransaction(property: model.PropertyTransaction,
                         contact: model.Contact,
                         tnc: model.TNC,
                         insertedClients: new List<PTOClientEntity>(),
                         updatedClients: ConvertToClient(model: model),
                         deletedClients: new List<PTOClientEntity>(),

                         insertedInternals: new List<PropertyTransactionDetailEntity>(),
                         updatedInternals: ConvertToSalesperson(model),
                         deletedInternals: new List<PropertyTransactionDetailEntity>(),

                         insertedExternals: new List<PropertyTransactionDetailEntity>(),
                         updatedExternals: new List<PropertyTransactionDetailEntity>(),
                         deletedExternals: new List<PropertyTransactionDetailEntity>(),

                         insertedExpense: new List<MarketingExpenseEntity>(),
                         updatedExpense: new List<MarketingExpenseEntity>(),
                         deletedExpense: new List<MarketingExpenseEntity>());

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.PropertyTransaction.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("reverse")]
        public IHttpActionResult Reverse(string syskey)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _propertyTransaction.ReverseAsTransaction(syskey);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(syskey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("void")]
        public IHttpActionResult Void(string syskey)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _propertyTransaction.VoidAsTransaction(syskey);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(syskey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utility

        private ReferralSalesOrderModel GetDefaultModel(string key)
        {
            var referralSalesperson = _propertyTransactionDetail.Finds(filter: f => f.PTOSK == key && f.TransType == (int)TransType.MainSalesperson).SingleOrDefault();
            return new ReferralSalesOrderModel
            {
                PropertyTransaction = _propertyTransaction.Finds(filter: f => f.SysKey == key).Single(),
                TNC = _tnc.Finds(filter: f => f.SysKey == key).Single(),
                Contact = _contact.Finds(filter: f => f.SysKey == key).SingleOrDefault() ?? new ContactEntity(),
                Client = _ptoClient.Finds(filter: f => f.PTOSK == key).Single(),
                Internal = _propertyTransactionDetail.Finds(filter: f => f.PTOSK == key).Single(),
            };
        }

        private ReferralSalesOrderModel GetDefaultModel()
        {
            Func<TaxEntity> companyTax = () =>
            {
                var company = _myCompany.Finds(filter: f => f.SysKey == "1").Single();
                var tax = _tax.Finds(filter: f => f.SysKey == company.TaxSK).SingleOrDefault() ?? new TaxEntity();

                return tax;
            };

            try
            {
                var di = DI.Current.Get<IEssAppRepository>();
                var tax = companyTax();

                return new ReferralSalesOrderModel
                {
                    Contact = new ContactEntity
                    {
                        Country = di.DefaultCountry
                    },
                    PropertyTransaction = new PropertyTransactionEntity
                    {
                        CURRSK = di.CURRSK,
                        CURRATE = di.CURRATE,
                        CURROP = di.CURROP,
                        TransType = (int)TransType.ReferralSalesOrder,
                        TaxSK = tax.SysKey,
                        TaxPercent = tax.TaxPercent,
                        BillToCoBroke = true,
                        ClientTransType = (int)TransType.Bank,
                        CommissionRate = 100.00M,
                        GrossCommission = 0,
                        TaxAmount = 0,
                        TotalAmount = 0,
                    },
                    Internal = new PropertyTransactionDetailEntity(),
                    Client = new PTOClientEntity(),
                    TNC = new TNC_Entity(),
                };
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private IEnumerable<PTOClientEntity> ConvertToClient(ReferralSalesOrderModel model)
        {
            try
            {
                return new List<PTOClientEntity>
                {
                    new PTOClientEntity
                    {
                        SysKey = model.Client.SysKey,
                        IsMain = true,
                        PTOSK = model.Client.PTOSK,
                        ClientSK = model.Client.ClientSK,
                        TransType = (int)TransType.Client,
                    }
                };
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private IEnumerable<PropertyTransactionDetailEntity> ConvertToSalesperson(
           ReferralSalesOrderModel model)
        {
            try
            {
                var emp = _employment.Finds(filter: f => f.SysKey == model.Internal.SPSK).Single();

                return new List<PropertyTransactionDetailEntity>
                {
                    new PropertyTransactionDetailEntity
                    {
                        SysKey = model.Internal.SysKey,
                        IsMain = true,
                        TransType = (int)TransType.MainSalesperson,

                        GrossComm = (model.PropertyTransaction.GrossCommission) ,
                        TaxSK = model.PropertyTransaction.TaxSK,
                        TaxPercent = model.PropertyTransaction.TaxPercent,
                        GSTValue = model.PropertyTransaction.TaxAmount ,
                        TotalPayable = model.PropertyTransaction.TotalAmount,

                        //ReferralPayout = model.Internal.ReferralPayout,
                        PTOSK = model.Internal.PTOSK,
                        SchemeDetailSK = emp.SchemeDetailSysKey,
                        CurrentCommission = model.Internal.CurrentCommission,

                        VendorSK = "1",
                        SPSK = model.Internal.SPSK,
                        CURRATE = model.PropertyTransaction.CURRATE,
                        CURROP = model.PropertyTransaction.CURROP,
                        CURRSK = model.PropertyTransaction.CURRSK,
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
