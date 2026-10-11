namespace FC.Api.AccountAreas.Areas.Controllers
{
    using FC.Api.AccountAreas.Areas.Accounts.Models;
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
    using FluentValidation;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Http;

    [RoutePrefix("CashbookTransaction")]
    public class CashbookTransactionController : BaseApiController
    {
        #region Fields

        private Contact _contact;
        private DownloadResult _downloadResult;
        private CashBookTransaction _cashbookTransaction;
        private KendoGridFilterTransalator _kendoGridFilter;
        private CashbookTransactionValidator _validator;
        private ICrystalReporter _crystal;
        private Person _person;
        private Company _company;
        private CashbookSetup _cashbookSetup;

        #endregion

        #region Ctor

        public CashbookTransactionController(CashBookTransaction cashbookTransaction,
            CashbookTransactionValidator validator,
            ICrystalReporter crystal,
            CashbookSetup cashbookSetup,
            Person person,
            Company company,
            Contact contact,
            DownloadResult downloadResult,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _company = company;
            _person = person;
            _contact = contact;
            _validator = validator;
            _crystal = crystal;
            _downloadResult = downloadResult;
            _cashbookTransaction = cashbookTransaction;
            _kendoGridFilter = kendoGridFilter;
            _cashbookSetup = cashbookSetup;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CashbookTransactionDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CashbookTransactionDTO>(h);

                return _cashbookTransaction.GetPager(currentPage:
                    currentPage,
                    pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CashBookTransactionEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CashBookTransactionEntity>(h);

                var a = _cashbookTransaction.Finds(filter: e).ToList();
                return a;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CashBookTransactionEntity Find(string key)
        {
            try
            {
                return _cashbookTransaction.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CashBookTransactionEntity Default()
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
        public IHttpActionResult Save(CashbookTransactionModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                var key = "";

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    key = _cashbookTransaction.InsertAsTransaction(insertedItems: model.insertedItems);
                    scope.Complete();
                }
                return Ok(_cashbookTransaction.Finds(filter: f => f.ParentId == key).First());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(CashbookTransactionModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                var key = "";
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                   key =  _cashbookTransaction.UpdateAsTransaction(insertedItems: model.insertedItems,
                        updatedItems: model.updatedItems,
                        deletedItems: model.deletedItems);

                    scope.Complete();
                }

                return Ok(_cashbookTransaction.Finds(filter: f => f.ParentId == key).First());
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
                    _cashbookTransaction.Delete(filter: f => f.ParentId == syskey);

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
        public IHttpActionResult Post(CashbookTransactionModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                var key = "";
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    key = _cashbookTransaction.PostAsTransaction(
                         insertedItems: model.insertedItems,
                         updatedItems: model.updatedItems,
                         deletedItems: model.deletedItems);

                    scope.Complete();
                }
                return Ok(_cashbookTransaction.Finds(filter: f => f.ParentId == key).First());
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
                    _cashbookTransaction.ReverseAsTransaction(syskey);

                    scope.Complete();
                }
                return Ok(_cashbookTransaction.Finds(filter: f => f.ParentId == syskey).First());
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
                    _cashbookTransaction.VoidAsTransaction(syskey);

                    scope.Complete();
                }
                return Ok(_cashbookTransaction.Finds(filter: f => f.ParentId == syskey).First());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("recap/{parentId}/{transType}")]
        public IEnumerable<BTableEntity> Recap(string parentId,
            int transType)
        {
            try
            {
                return _cashbookTransaction.RecapAsTransaction(parentId: parentId,
                    transType: transType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("Pdf/{CBSK}/{SPSK}")]
        public IHttpActionResult Pdf(string CBSK, string SPSK)
        {
            try
            {
                PoppulatePrinting(cbsk: CBSK, spsk: SPSK);

                return _downloadResult.Excute(_crystal.ExportToStream(), Request,
                    "Cashbook Transaction.pdf");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void PoppulatePrinting(string cbsk,string spsk)
        {
            try
            {
                var entity = _cashbookTransaction.Finds(filter: f => f.SysKey == cbsk).First();
                var secondRef = entity.R1;
                var person = _person.Finds(filter: f => f.SysKey == spsk).Single();

                var spContact = person.TransType == (int)TransType.InternalSalesperson ?
                    _contact.Finds(filter: f => f.SysKey == person.SysKey).Single() :
                    _contact.Finds(filter: f => f.SysKey == person.VendorSK).Single();

                var company = _company.Finds(filter: f => f.SysKey == "1").Single();
                var companyContact = _contact.Finds(filter: f => f.SysKey == "1").Single();

                var cbCompany = _company.Finds(filter: f => f.SysKey == person.VendorSK).Single();
                var cashBook = _cashbookSetup.Finds(filter: f => f.SysKey == entity.CBSK).Single();
                var companyAddress = companyContact.StraightAddress();
                var companyTel = companyContact.HomeTelephone;
                var companyFax = companyContact.Fax;

                _crystal.Configurator.RptFileName = "RPT_005.rpt";

                #region formula fields difination

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
                _crystal.SetFormulaField("NRIC", person.NRIC);
                //INVNumber
                _crystal.SetFormulaField("INVNumber", entity.SecondRef);
                //_crystal.SetFormulaField("SalespersonType", person.TransType.ToString());
                _crystal.SetFormulaField("spName", person.Name);
                _crystal.SetFormulaField("Mobile", spContact.MobilePhone);

                if (cashBook.CBType == (int)CashBookType.Cash)
                    _crystal.SetFormulaField("BankName", CashBookType.Cash.ToDescription());
                else
                    _crystal.SetFormulaField("BankName", cashBook.Description);

                _crystal.SetFormulaField("SecondRef", secondRef);
                _crystal.SetFormulaField("PVNumber", entity.R1);
                _crystal.SetFormulaField("PVDate", GeneralUtility.ConvertDisplayDateFormat(entity.TransactionDate));

                #endregion

                var ds = new DataSet();

                var td = _cashbookTransaction.Finds(filter: f => f.ParentId == entity.ParentId)
                    .Select(x=> new { x.AccCode , x.Memo , x.TransactionAmount })
                    .ToDataTable("CashbookMemoDataTable");

                ds.Tables.Add(td);

                _crystal.SetDataSource(ds);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utility

        private CashBookTransactionEntity GetDefaultModel()
        {
            return new CashBookTransactionEntity
            {
                R1 = CommonConstant.DefaultIndexing.R1,
                RecordStatus = (int)RecordStatus.New,
                TransType = (int)TransType.CashBookReceived,
            };
        }

        #endregion
    }

    public class PrintingModel
    {
        public string SPSK { get; set; }
        public string CBSK { get; set; }
    }
}
