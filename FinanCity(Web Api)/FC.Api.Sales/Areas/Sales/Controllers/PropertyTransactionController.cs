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
  
    [RoutePrefix("PropertyTransaction")]
    public class PropertyTransactionController : BaseApiController
    {
        #region Fields

        private TNC _tnc;
        private KendoGridFilterTransalator _kendoGridFilter;
        private PropertyTransaction _propertyTransaction;
        private PropertyTransactionDetail _propertyTransactionDetail;
        private Contact _contact;
        private PTOClient _client;
        private PropertyTransactionValidator _validator;
        private Company _myCompany;
        private Tax _tax;
        private MarketingExpense _promotionItemExpense;
      
        #endregion

        #region Ctor

        public PropertyTransactionController(
            MarketingExpense promotionItemExpense,
            Company myCompany,
            Tax tax,
            TNC tnc,
            PropertyTransaction propertyTransaction,
            PropertyTransactionValidator validator,
            PropertyTransactionDetail propertyTransactionDetail,
            Contact contact,
            KendoGridDataCollector<PropertyTransactionDetailEntity> internalGridDataCollector,
            PTOClient client,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _promotionItemExpense = promotionItemExpense;
            _tax = tax;
            _myCompany = myCompany;
            _validator = validator;
            _tnc = tnc;
            _propertyTransactionDetail = propertyTransactionDetail;
            _client = client;
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
                e = e.Constant((x) => x.TransType == (int)TransType.PropertyTransaction);

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

                e = e.Constant((x) => x.TransType == (int)TransType.PropertyTransaction);

                e.CreateObject((mem, value) =>
                {
                    if (mem == $"{nameof(PropertyTransactionEntity.SysKey)}")
                    {
                        if (value != null)
                        {
                            var key = _propertyTransaction.GetSingleKey(value);
                            e = e.Constant(c => c.SysKey == key);
                        }
                    }
                });

                return _propertyTransaction.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public PropertyTransactionModel Find(string key)
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
        public PropertyTransactionModel Default()
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
        [Route("savedraft")]
        public IHttpActionResult SaveDraft(PropertyTransactionCreateOrUpdateModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.Property.RecordStatus = (int)RecordStatus.SaveDraft;
                    model.Property = _propertyTransaction.InsertAsTransaction(property: model.Property,
                        content: model.Contact,
                        tnc: model.TNC,
                        clients: model.ClientInsertedItems,
                        internals: model.InternalInsertedItems,
                        externals: model.ExternalInsertedItems,
                        insertedExpense: model.PromotionItemExpenseInsertedItems);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.Property.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("submit")]
        public IHttpActionResult Submit(PropertyTransactionCreateOrUpdateModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.Property.RecordStatus = (int)RecordStatus.Submit;
                    _propertyTransaction.UpdateAsTransaction(property: model.Property,
                          contact: model.Contact,
                          tnc: model.TNC,
                          insertedClients: model.ClientInsertedItems,
                          updatedClients: model.ClientUpdatedItems,
                          deletedClients: model.ClientDeletedItems,

                          insertedInternals: model.InternalInsertedItems,
                          updatedInternals: model.InternalUpdatedItems,
                          deletedInternals: model.InternalDeletedItems,

                          insertedExternals: model.ExternalInsertedItems,
                          updatedExternals: model.ExternalUpdatedItems,
                          deletedExternals: model.ExternalDeletedItems,

                          insertedExpense: model.PromotionItemExpenseInsertedItems,
                          updatedExpense: model.PromotionItemExpenseUpdatedItems,
                          deletedExpense: model.PromotionItemExpenseDeletedITems);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.Property.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("approve")]
        public IHttpActionResult Approve(PropertyTransactionCreateOrUpdateModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.Property.RecordStatus = (int)RecordStatus.Save;
                    _propertyTransaction.UpdateAsTransaction(property: model.Property,
                         contact: model.Contact,
                         tnc: model.TNC,
                         insertedClients: model.ClientInsertedItems,
                         updatedClients: model.ClientUpdatedItems,
                         deletedClients: model.ClientDeletedItems,

                         insertedInternals: model.InternalInsertedItems,
                         updatedInternals: model.InternalUpdatedItems,
                         deletedInternals: model.InternalDeletedItems,

                         insertedExternals: model.ExternalInsertedItems,
                         updatedExternals: model.ExternalUpdatedItems,
                         deletedExternals: model.ExternalDeletedItems,

                         insertedExpense: model.PromotionItemExpenseInsertedItems,
                         updatedExpense: model.PromotionItemExpenseUpdatedItems,
                         deletedExpense: model.PromotionItemExpenseDeletedITems);

                    scope.Complete();

                }

                return Ok(GetDefaultModel(model.Property.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("reject")]
        public IHttpActionResult Reject(PropertyTransactionCreateOrUpdateModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.Property.RecordStatus = (int)RecordStatus.Reject;
                    _propertyTransaction.UpdateAsTransaction(property: model.Property,
                         contact: model.Contact,
                         tnc: model.TNC,
                         insertedClients: model.ClientInsertedItems,
                         updatedClients: model.ClientUpdatedItems,
                         deletedClients: model.ClientDeletedItems,

                         insertedInternals: model.InternalInsertedItems,
                         updatedInternals: model.InternalUpdatedItems,
                         deletedInternals: model.InternalDeletedItems,

                         insertedExternals: model.ExternalInsertedItems,
                         updatedExternals: model.ExternalUpdatedItems,
                         deletedExternals: model.ExternalDeletedItems,

                         insertedExpense: model.PromotionItemExpenseInsertedItems,
                         updatedExpense: model.PromotionItemExpenseUpdatedItems,
                         deletedExpense: model.PromotionItemExpenseDeletedITems);

                    scope.Complete();

                }

                return Ok(GetDefaultModel(model.Property.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(PropertyTransactionCreateOrUpdateModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.Property = _propertyTransaction.InsertAsTransaction(property: model.Property,
                        content: model.Contact,
                        tnc: model.TNC,
                        clients: model.ClientInsertedItems,
                        internals: model.InternalInsertedItems,
                        externals: model.ExternalInsertedItems,
                        insertedExpense: model.PromotionItemExpenseInsertedItems);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.Property.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(PropertyTransactionCreateOrUpdateModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _propertyTransaction.UpdateAsTransaction(property: model.Property,
                         contact: model.Contact,
                         tnc: model.TNC,
                         insertedClients: model.ClientInsertedItems,
                         updatedClients: model.ClientUpdatedItems,
                         deletedClients: model.ClientDeletedItems,

                         insertedInternals: model.InternalInsertedItems,
                         updatedInternals: model.InternalUpdatedItems,
                         deletedInternals: model.InternalDeletedItems,

                         insertedExternals: model.ExternalInsertedItems,
                         updatedExternals: model.ExternalUpdatedItems,
                         deletedExternals: model.ExternalDeletedItems,
                         
                         insertedExpense: model.PromotionItemExpenseInsertedItems,
                         updatedExpense: model.PromotionItemExpenseUpdatedItems,
                         deletedExpense: model.PromotionItemExpenseDeletedITems);

                    scope.Complete();
                }
                return Ok(GetDefaultModel(model.Property.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("post")]
        public IHttpActionResult Post(PropertyTransactionCreateOrUpdateModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _propertyTransaction.PostAsTransaction(property: model.Property,
                          contact: model.Contact,
                          tnc: model.TNC,
                          insertedClients: model.ClientInsertedItems,
                          updatedClients: model.ClientUpdatedItems,
                          deletedClients: model.ClientDeletedItems,

                          insertedInternals: model.InternalInsertedItems,
                          updatedInternals: model.InternalUpdatedItems,
                          deletedInternals: model.InternalDeletedItems,

                          insertedExternals: model.ExternalInsertedItems,
                          updatedExternals: model.ExternalUpdatedItems,
                          deletedExternals: model.ExternalDeletedItems,

                          insertedExpense: model.PromotionItemExpenseInsertedItems,
                          updatedExpense: model.PromotionItemExpenseUpdatedItems,
                          deletedExpense: model.PromotionItemExpenseDeletedITems);

                    scope.Complete();
                }
                return Ok(GetDefaultModel(model.Property.SysKey));
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

        private PropertyTransactionModel GetDefaultModel(string key)
        {
            //key = _propertyTransaction.GetSingleKey(key);
            return new PropertyTransactionModel
            {
                PropertyTransaction = _propertyTransaction.Finds(filter: f => f.SysKey == key).SingleOrDefault(),
                Contact = _contact.Finds(filter: f => f.SysKey == key).SingleOrDefault() ?? new ContactEntity(),
                Clients = _client.Finds(filter: f => f.PTOSK == key),
                Internals = _propertyTransactionDetail.Finds(filter: f => f.PTOSK == key && (f.TransType == (int)TransType.InternalSalesperson || f.TransType == (int)TransType.MainSalesperson)),
                Externals = _propertyTransactionDetail.Finds(filter: f => f.PTOSK == key && f.TransType == (int)TransType.ExternalSalesPerson),
                TNC = _tnc.Finds(filter: f => f.SysKey == key).Single(),
                PromotionExpense = _promotionItemExpense.Finds(filter: f => f.PTOSK == key),
            };
        }

        private PropertyTransactionModel GetDefaultModel()
        {
            Func<TNC_Entity> defaultTNC = () =>
            {
                return new TNC_Entity
                {
                    TransactionTypeSK = "2017042514462609135",
                    TransactionTypeCompleteStatus = true,
                    PropertyTypeSK = "2017042514462609135",
                    PropertyPurposeSK = "2017042514462609135",
                    InterestRateTypeSK = "2017042514462609135",
                    RateTypeSK = "2017042514462609135",
                    RateTypeMargin = 0,
                    LoanAmount = 0,
                };
            };

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

                return new PropertyTransactionModel
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
                        TransType = (int)TransType.PropertyTransaction,
                        TaxSK = tax.SysKey,
                        TaxPercent = tax.TaxPercent,
                        BillToCoBroke = true,
                        ClientTransType = (int)TransType.Bank,
                        CommissionRate = 0.15M,
                        GrossCommission = 0,
                        TaxAmount = 0,
                        TotalAmount = 0,
                    },
                    Clients = new List<PTOClientEntity>(),
                    Internals = new List<PropertyTransactionDetailEntity>(),
                    Externals = new List<PropertyTransactionDetailEntity>(),
                    TNC = defaultTNC(),
                    PromotionExpense = new List<MarketingExpenseEntity>(),
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
