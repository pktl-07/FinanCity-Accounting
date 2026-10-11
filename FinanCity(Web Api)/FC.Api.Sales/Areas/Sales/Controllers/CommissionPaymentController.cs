namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using FC.Core;
    using FC.Core.Data;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FluentValidation;

    [RoutePrefix("CommissionPayment")]
    public class CommissionPaymentController : BaseApiController
    {
        #region Fields

        private DownloadResult _downloadResult;
        private Company _company;
        private CashbookSetup _cashbookSetup;
        private ICrystalReporter _crystal;
        private CommissionPayment _commissionPayment;
        private KendoGridFilterTransalator _kendoGridFilter;
        private CommissionPayableValidator _validator;
        private CommissionAllocation _allocation;
        private SalesInvoice _salesInvoice;
        private PropertyTransaction _propertyTransaction;
        private Contact _contact;
        private Person _person;
        private PropertyTransactionDetail _propertyTransactionDetail;
        private PayableScheme _payableScheme;

        #endregion

        #region Ctor

        public CommissionPaymentController(CommissionPayment commissoinPayment,
            Company company,
            DownloadResult downloadResult,
            PayableScheme payableScheme,
            CashbookSetup cashbookSetup,
            CommissionPayableValidator validator,
            ICrystalReporter crystal,
            PropertyTransactionDetail propertyTransactionDetail,
            Contact contact,
            Person person,
            PropertyTransaction propertyTransaction,
            SalesInvoice salesInvoice,
            CommissionAllocation allocation,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _downloadResult = downloadResult;
            _payableScheme = payableScheme;
            _company = company;
            _cashbookSetup = cashbookSetup;
            _crystal = crystal;
            _propertyTransactionDetail = propertyTransactionDetail;
            _person = person;
            _contact = contact;
            _propertyTransaction = propertyTransaction;
            _salesInvoice = salesInvoice;
            _allocation = allocation;
            _validator = validator;
            _commissionPayment = commissoinPayment;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CommissionPayableDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionPayableDTO>(h);
          
                return _commissionPayment.GetPager(currentPage: currentPage, pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CommissionPaymentEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionPaymentEntity>(h);

                return _commissionPayment.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CommissionPaymentEntity Find(string key)
        {
            try
            {
                return _commissionPayment.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CommissionPaymentEntity Default()
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
        public IHttpActionResult Save(CommissionPaymentEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model = _commissionPayment.Insert(model);

                    scope.Complete();
                }

                return Ok(_commissionPayment.Finds(filter: f => f.SysKey == model.SysKey).SingleOrDefault());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(CommissionPaymentEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _commissionPayment.Update(entity: model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();
                }

                return Ok(_commissionPayment.Finds(filter: f => f.SysKey == model.SysKey).SingleOrDefault());
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
                    _commissionPayment.Delete(filter: f => f.SysKey == syskey);

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
        public IHttpActionResult Post(CommissionPaymentEntity model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _commissionPayment.PostAsTransaction(commissionPayment: model);

                    scope.Complete();
                }

                return Ok(_commissionPayment.Finds(filter: f => f.SysKey == model.SysKey).Single());
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
                    _commissionPayment.ReverseAsTransaction(syskey);
                    scope.Complete();
                }
                return Ok(_commissionPayment.Finds(filter: f => f.SysKey == syskey).Single());
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
                    _commissionPayment.VoidAsTransaction(syskey);

                    scope.Complete();
                }
                return Ok(_commissionPayment.Finds(filter: f => f.SysKey == syskey).Single());
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
                return _commissionPayment.RecapAsTransaction(key: key);
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
                var e = _kendoGridFilter.Convert<CommissionPaymentEntity>(h);
                var key = "";

                e.CreateObject((member, data) =>
                {
                    if (member == nameof(CommissionPaymentEntity.SysKey))
                        key = data;
                });

                PoppulatePrinting(key: key);

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

        private CommissionPaymentEntity GetDefaultModel()
        {
            return new CommissionPaymentEntity 
            {
                TransType = (int)TransType.CommissionPayable,
                PaymentType = (int)PaymentType.Full,
            };
        }

        private void PoppulatePrinting(string key)
        {
            try
            {
                var entity = _commissionPayment.Finds(filter: f => f.SysKey == key).Single();
                var allocation = _allocation.Finds(filter: f => f.SysKey == entity.ALOSK).Single();
                var invoice = _salesInvoice.Finds(filter: f => f.SysKey == allocation.HeaderSK).Single();
                var propertyTransaction = _propertyTransaction.Finds(filter: f => f.SysKey == allocation.HeaderSK).Single();
                var secondRef = propertyTransaction.R1;
                var propertyContact = _contact.Finds(filter: f => f.SysKey == invoice.SysKey).Single();
                var person = _person.Finds(filter: f => f.SysKey == allocation.SPSK).Single();

                var spContact = person.TransType == (int)TransType.InternalSalesperson ?
                    _contact.Finds(filter: f => f.SysKey == person.SysKey).Single() :
                    _contact.Finds(filter: f => f.SysKey == person.VendorSK).Single();
               
                var company = _company.Finds(filter: f => f.SysKey == "1").Single();
                var companyContact = _contact.Finds(filter: f => f.SysKey == "1").Single();

                var gstPercent = _propertyTransactionDetail.Finds(filter: f => f.SPSK == allocation.SPSK
                 && f.PTOSK == allocation.HeaderSK).Single().TaxPercent;

                allocation.GSTValue = ((entity.NetPaid.ToRoundUp() / (100 + gstPercent)) * gstPercent).ToRoundUp();

                var cbCompany = _company.Finds(filter: f => f.SysKey == person.VendorSK).Single();
                var cashBook = _cashbookSetup.Finds(filter: f => f.SysKey == entity.CBSK).Single();
                var companyAddress = companyContact.StraightAddress();
                var companyTel = companyContact.HomeTelephone;
                var companyFax = companyContact.Fax;

                _crystal.Configurator.Attachment.Title = $"Commission Payment For {entity.R1}";

                if (person.TransType != (int)TransType.InternalSalesperson)
                    _crystal.Configurator.RptFileName = "RPT_004_1.rpt";
                else
                    _crystal.Configurator.RptFileName = "RPT_004.rpt";

                #region formula fields difination

                var propertyAddress = propertyContact.SetFullAddress();

                var spAddress = spContact.SetFullAddress();

                switch (entity.RecordStatus)
                {
                    case (int)RecordStatus.Save:
                    case (int)RecordStatus.Update:

                        _crystal.SetFormulaField("Title", "Acknowledgement of Payment");

                        break;
                    case (int)RecordStatus.Post:

                        _crystal.SetFormulaField("Title", "Payment Voucher");

                        break;
                }

                _crystal.SetFormulaField("CompanyName", company.CompanyName);
                _crystal.SetFormulaField("CompanyAddress", companyAddress);
                _crystal.SetFormulaField("CompanyTelephone", companyTel);
                _crystal.SetFormulaField("CompanyFax", companyFax);
                _crystal.SetFormulaField("RegNo", company.CompanyReg);
                _crystal.SetFormulaField("Address", propertyAddress);
                _crystal.SetFormulaField("NRIC", person.NRIC);
                _crystal.SetFormulaField("spType", person.TransType.ToString());
                _crystal.SetFormulaField("spName", person.Name);
                _crystal.SetFormulaField("Mobile", spContact.MobilePhone);
            
                if (cashBook.CBType == (int)CashBookType.Cash)
                    _crystal.SetFormulaField("bankName", CashBookType.Cash.ToDescription());
                else
                    _crystal.SetFormulaField("bankName", cashBook.Description);
              
                _crystal.SetFormulaField("SecondRef", secondRef);

                if (person.TransType == (int)TransType.InternalSalesperson)
                {
                    _crystal.SetFormulaField("INVNumber", invoice.R1);
                    _crystal.SetFormulaField("PVNumber", entity.R1);
                    _crystal.SetFormulaField("PVDate", GeneralUtility.ConvertDisplayDateFormat(entity.DatePaid));
                }

                #endregion

                var dataSet = new DataSet();
             
                dataSet.Tables.Add(_commissionPayment.CommissionPaymentPrinting(key));

                _crystal.SetDataSource(dataSet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
