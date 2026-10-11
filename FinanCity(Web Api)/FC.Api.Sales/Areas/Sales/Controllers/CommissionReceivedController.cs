namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("CommissionReceived")]
    public class CommissionReceivedController : BaseApiController
    {
        #region Field

        private ICrystalReporter _crystal;
        private CommissionReceived _commissionReceived;
        private SalesInvoice _salesInvoice;
        private KendoGridFilterTransalator _kendoGridFilter;
        private CommissionReceivableValidator _validator;
        private CashbookSetup _cashbookSetup;
        private DownloadResult _downloadResult;
        private PropertyTransaction _propertyTransaction;
        private Contact _contact;
        private Company _company;
        private Person _person;

        #endregion

        #region Ctor

        public CommissionReceivedController(
            Person person,
            Company company,
            Contact contact,
            DownloadResult downloadResult,
            PropertyTransaction propertyTransaction,
            CashbookSetup cashbookSetup,
            ICrystalReporter crystal,
            CommissionReceivableValidator validator,
            SalesInvoice salesInvoice,
            CommissionReceived commissionReceived,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _person = person;
            _company = company;
            _contact = contact;
            _propertyTransaction = propertyTransaction;
            _cashbookSetup = cashbookSetup;
            _crystal = crystal;
            _validator = validator;
            _salesInvoice = salesInvoice;
            _commissionReceived = commissionReceived;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CommissionReceivedDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionReceivedDTO>(h);

                return _commissionReceived.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CommissionReceivedEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionReceivedEntity>(h);

                return _commissionReceived.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CommissionReceivedEntity Find(string key)
        {
            try
            {
                return _commissionReceived.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CommissionReceivedEntity Default()
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
        public IHttpActionResult Save(CommissionReceivedEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    model = _commissionReceived.Insert(model);

                    scope.Complete();
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(CommissionReceivedEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    _commissionReceived.Update(entity: model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();
                }

                return Ok(_commissionReceived.Finds(filter: f => f.SysKey == model.SysKey).Single());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult Delete(string syskey)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _commissionReceived.Delete(filter: f => f.SysKey == syskey);

                    scope.Complete();
                }
                return Ok(GetDefaultModel());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("post")]
        public IHttpActionResult Post(CommissionReceivedEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    _commissionReceived.PostAsTransaction(model);

                    scope.Complete();
                }

                return Ok(_commissionReceived.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _commissionReceived.ReverseAsTransaction(syskey);

                    scope.Complete();
                }

                return Ok(_commissionReceived.Finds(filter: f => f.SysKey == syskey).Single());
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
                    _commissionReceived.VoidAsTransaction(syskey);

                    scope.Complete();
                }

                return Ok(_commissionReceived.Finds(filter: f => f.SysKey == syskey).Single());
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
                return _commissionReceived.RecapAsTransaction(key: key);
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
                var e = _kendoGridFilter.Convert<CommissionReceivedEntity>(h);
                var key = "";

                e.CreateObject((member, data) =>
                {
                    if (member == nameof(CommissionReceivedEntity.SysKey))
                    {
                        key = data;
                    }
                });

                PoppulateCommisionReceivableReport(key: key);

                return _downloadResult.Excute(_crystal.ExportToStream(), Request,
                    _crystal.Configurator.Attachment.Title + ".pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utility

        private CommissionReceivedEntity GetDefaultModel()
        {
            return new CommissionReceivedEntity
            {
                PaymentMode = (int)PaymentMode.CHEQUE,
                PaymentType = (int)PaymentType.Full,
            };
        }

        private void PoppulateCommisionReceivableReport(string key)
        {
            var received = _commissionReceived.Finds(filter: f => f.SysKey == key).Single();

            _crystal.Configurator.RptFileName = "RPT_003.rpt";
            _crystal.Configurator.Attachment.Title = $"Commission Recipt For {received.R1}";

            var invoice = _salesInvoice.Finds(filter: f => f.SysKey == received.InvoiceSK).Single();
            var property = _propertyTransaction.Finds(filter: f => f.SysKey == received.InvoiceSK).Single();
            var company = _company.Finds(filter: f => f.SysKey == "1").Single();
            var cashOrCheque = "";

            switch (_cashbookSetup.Finds(filter: f => f.SysKey == received.CBSK).Single().CBType)
            {
                case 1:
                    cashOrCheque = "CASH";
                    break;
                case 2:
                    cashOrCheque = received.BankName + received.ChequeNo;
                    break;
            }

            var clientSK = property.MailingClientSK;
            var clientContact = _contact.Finds(filter: f => f.SysKey == clientSK).Single();
            var companyContact = _contact.Finds(filter: f => f.SysKey == "1").Single();
            var propertyContact = _contact.Finds(filter: f => f.SysKey == invoice.SysKey).Single();

            if (property.BillToCoBroke)
            {
                var person = _person.Finds(filter: f => f.SysKey == property.MailingClientSK).Single();
                var coBrokeAgency = _company.Finds(filter: f => f.SysKey == person.VendorSK).Single();
                clientContact = _contact.Finds(filter: f => f.SysKey == coBrokeAgency.SysKey).Single();
            }

            #region formula fields difination

            var propertyAddress = propertyContact.SetFullAddress();
            var clientAddress = clientContact.SetFullAddress();
            var companyAddress = companyContact.StraightAddress();
            var companyTel = companyContact.HomeTelephone;
            var companyFax = companyContact.Fax;

            switch (received.RecordStatus)
            {
                case (int)RecordStatus.Save:
                case (int)RecordStatus.Update:
                    _crystal.SetFormulaField("Title", "Acknowledgement of Receipt");
                    break;

                case (int)RecordStatus.Post:
                    _crystal.SetFormulaField("Title", "Official Receipt");
                    break;
            }

            _crystal.SetFormulaField("CompanyName", company.CompanyName);
            _crystal.SetFormulaField("CompanyAddress", companyAddress);
            _crystal.SetFormulaField("CompanyTelephone", companyTel);
            _crystal.SetFormulaField("CompanyFax", companyFax);
            _crystal.SetFormulaField("RegNo", "GST Reg. No. " + company.CompanyReg);
            _crystal.SetFormulaField("ClientAddress", clientAddress);
            _crystal.SetFormulaField("PropertyAddress", propertyAddress);
            _crystal.SetFormulaField("CashOrCheque", cashOrCheque);
            _crystal.SetFormulaField("SubTotal", (received.AmountBeforeGST).ToRoundUp().ToString("N", new CultureInfo("en-us")));
            _crystal.SetFormulaField("GSTPercent", ((decimal)received.GSTPercent).ToString("0.00"));
            _crystal.SetFormulaField("GSTValue", (received.GSTValue).ToRoundUp().ToString("N", new CultureInfo("en-us")));
            _crystal.SetFormulaField("SecondRef", "");
            _crystal.SetFormulaField("BankName", received.BankName);
            _crystal.SetDataSource(_commissionReceived.CommissionReceivedTemplateDataTable(key: key));

            #endregion
        }

        #endregion
    }
}
