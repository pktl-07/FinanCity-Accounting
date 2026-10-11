namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Accounts.Entity;
    using FC.Core.Infrastructure.Implements.RecordIndexer;

    [RoutePrefix("SalesInvoice")]
    public class SalesInvoiceController : BaseApiController
    {
        #region Fields

        private ICrystalReporter _crystal;
        private KendoGridFilterTransalator _kendoGridFilter;
        private SalesInvoice _salesInvoice;
        private SalesInvoiceValidator _validator;
        private Contact _contact;
        private DownloadResult _downloadResult;
        private Company _company;
        private PropertyTransaction _propertyTransaction;
        private Person _person;
        private Currency _currency;
        private IEssAppRepository _app;
        private RecordIndexerFactory _recordIndexer;
        private PTOClient _ptoClient;
      
      
        #endregion

        #region Ctor

        public SalesInvoiceController(
            IEssAppRepository app,
            RecordIndexerFactory recordIndexer,
            PropertyTransaction propertyTransaction,
            Company company,
            Currency currency,
            ICrystalReporter crystal,
            SalesInvoice salesInvoice,
            DownloadResult downloadResult,
            Contact contact,
            SalesInvoiceValidator validator,
            KendoGridFilterTransalator kendoGridFilter,
            PTOClient ptoClient,
            Person person)
        {
            _app = app;
            _currency = currency;
            _person = person;
            _propertyTransaction = propertyTransaction;
            _company = company;
            _crystal = crystal;
            _downloadResult = downloadResult;
            _contact = contact;
            _validator = validator;
            _salesInvoice = salesInvoice;
            _kendoGridFilter = kendoGridFilter;
            _recordIndexer = recordIndexer;
            _ptoClient = ptoClient;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<InvoiceDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<InvoiceDTO>(h);
               
                return _salesInvoice.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<SalesInvoiceEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<SalesInvoiceEntity>(h);

                return _salesInvoice.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public SalesInvoiceModel Find(string key)
        {
            try
            {
                var copy = (string)key.Clone();
                key = _salesInvoice.GetSingleKey(key);

                return new SalesInvoiceModel
                {
                    SalesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == copy).SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == key).SingleOrDefault(),
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(SalesInvoiceModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    var customIndexing = _salesInvoice.GetCustomIndexingContext();

                    model.SalesInvoice.R1 = customIndexing.R1;
                    model.SalesInvoice.Prefix = customIndexing.Prefix;
                    model.SalesInvoice.Postfix = customIndexing.Postfix;
                    model.SalesInvoice.VoucherId = customIndexing.VoucherId;
                    model.SalesInvoice.RecordStatus = (int)RecordStatus.Update;

                    _salesInvoice.UpdateAsTransaction(salesInvoice: model.SalesInvoice,
                        contact: model.Contact);

                    scope.Complete();
                }

                var key = _salesInvoice.GetSingleKey(model.SalesInvoice.SysKey);
                return Ok(new SalesInvoiceModel
                {
                    SalesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == key).SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == key).SingleOrDefault(),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(SalesInvoiceModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    var recordStatus = model.SalesInvoice.RecordStatus == (int)RecordStatus.Post ?
                        (RecordStatus)model.SalesInvoice.RecordStatus : RecordStatus.Update;

                    _salesInvoice.UpdateAsTransaction(salesInvoice: model.SalesInvoice,
                        contact: model.Contact,
                        recordStatus);

                    scope.Complete();
                }

                return Ok(new SalesInvoiceModel
                {
                    SalesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == model.SalesInvoice.SysKey).SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == model.SalesInvoice.SysKey).SingleOrDefault(),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("post")]
        public IHttpActionResult Post(SalesInvoiceModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _salesInvoice.PostAsTransaction(salesInvoice: model.SalesInvoice,
                        contact: model.Contact);

                    scope.Complete();
                }

                return Ok(new SalesInvoiceModel
                {
                    SalesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == model.SalesInvoice.SysKey).SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == model.SalesInvoice.SysKey).SingleOrDefault(),
                });
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
                    _salesInvoice.ReverseAsTransaction(syskey);
                    scope.Complete();
                }

                return Ok(new SalesInvoiceModel
                {
                    SalesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == syskey).SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == syskey).SingleOrDefault(),
                });
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
                    _salesInvoice.VoidAsTransaction(syskey);
                    scope.Complete();
                }

                return Ok(new SalesInvoiceModel
                {
                    SalesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == syskey).SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == syskey).SingleOrDefault(),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("recap/{key}")]
        public IEnumerable<BTableEntity> Recap(string key)
        {
            try
            {
                return _salesInvoice.RecapAsTransaction(key: key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("Pdf")]
        public IHttpActionResult Pdf()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<SalesInvoiceEntity>(h);
                var key = "";

                e.CreateObject((member,data) =>
                {
                    if (member == nameof(SalesInvoiceEntity.SysKey))
                    {
                        key = data;
                    }
                });

                PoppulateInvoiceReport(key: key);

                return _downloadResult.Excute(_crystal.ExportToStream(), Request,
                    _crystal.Configurator.Attachment.Title + ".pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utilities

        private void PoppulateInvoiceReport(string key)
        {
            try
            {
                key = _salesInvoice.GetSingleKey(key);

                var company = _company.Finds(filter: f => f.SysKey == "1").Single();
                var localContact = _contact.Finds(filter: f => f.SysKey == "1").Single();
                var vendorEntity = new CompanyEntity();
                var mailingAddress = "";
                var vendorContact = new ContactEntity();
                var mailingContact = new ContactEntity();
                var salesOrder = _propertyTransaction.Finds(filter: f => f.SysKey == key).Single();
                var salesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == key).Single();
                var propertyContact = _contact.Finds(filter: f => f.SysKey == key).Single();

                var bankerSK = _person.Finds(filter: f => f.SysKey == salesOrder.MailingClientSK).Single().VendorSK;
                var banker = _company.Finds(filter: f => f.SysKey == bankerSK).Single();


                var clientSK = _ptoClient.Finds(filter: f => f.PTOSK == key && f.IsMain == true).Single().ClientSK;
                var mainClient = _person.Finds(filter: f => f.SysKey == clientSK).Single();
                var currsk = _app.CURRSK;
                var currency = _currency.Finds(filter: f => f.SysKey == currsk).Single();

                mailingAddress = localContact.StraightAddress();
                mailingContact = _contact.Finds(filter: f => f.SysKey == key).Single();

                var clientAddress = mailingContact.SetFullAddress();
                var mainClientName = mainClient.Name;

                _crystal.Configurator.Attachment.Title = "Sales Invoice";
                _crystal.Configurator.RptFileName = "RPT_001_1.rpt";
                _crystal.SetDataSource(_salesInvoice.InvoiceTemplateDataTable(invoicesk: key));

                var companyTel = localContact.HomeTelephone;
                var companyFax = localContact.Fax;

                _crystal.SetFormulaField("CURRCODE", _app.CURRCODE);
                _crystal.SetFormulaField("CURRSYMBOL", "S$");
                _crystal.SetFormulaField("CompanyName", company.CompanyName);
                _crystal.SetFormulaField("RegNo", company.CompanyReg);

                _crystal.SetFormulaField("BankerFor", banker.CompanyName);
                _crystal.SetFormulaField("Header", salesInvoice.RecordStatus == (int)RecordStatus.Post ? "Sales Invoice" : "Performa Invoice");
                _crystal.SetFormulaField("CompanyTelephone", companyTel);
                _crystal.SetFormulaField("CompanyFax", companyFax);
                _crystal.SetFormulaField("SecondRef", salesOrder.SecondRef);
                _crystal.SetFormulaField("SecondaryTemplate", ((int)TransType.Client).ToString());

                var vendorAdddress = vendorContact.SetFullAddress();
                var propertyAddress = propertyContact.SetFullAddress().ToUpper();

                _crystal.SetFormulaField("PropertyAddress", propertyAddress);
                _crystal.SetFormulaField("MailingAddress", mailingAddress);
                _crystal.SetFormulaField("ClientName", mainClientName); // FOR JUST RPT_001_1 ONLY
                _crystal.SetFormulaField("ClientAddress", clientAddress);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
