namespace FC.Api.Controllers
{
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Sales.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;

    [RoutePrefix("utility")]
    public class UtilityController : BaseApiController
    {
        #region Fields

        private Notification _notification;
       // private RecursiveSalesBonusBuilder _recursiveSalesBonusBuilder;
        private Contact _contact;
        private Person _person;
        private MarketingExpense _marketingExpense;
        private CommissionAllocation _allocation;
        private CommissionHelper _commissionHelper;
        private Employment _employment;
        private CommissionStructureDetail _commSchemeDetail;
        private KendoGridFilterTransalator _kendoGridFilter;
        private CommissionPayment _commissionPayment;
        private AccountingPeriod _accountingPeriod;
        private CareerManagement _careerManagement;
        private IFinanCityIdentity _identity;
        private Chatting _chatting;
        private OverridingProfileItem _overridingProfileItem;
        #endregion

        #region Ctor

        public UtilityController(Person person,
            Notification notification,
            OverridingProfileItem overridingProfileItem,
            CommissionPayment commissionPayment,
            MarketingExpense marketingExpense,
            KendoGridFilterTransalator kendoGridFilter,
            CommissionAllocation allocation,
            Employment employment,
            AccountingPeriod accountingPeriod,
            CommissionStructureDetail commSchemeDetail,
            CareerManagement careerManagement,
            Contact contact,
            CommissionHelper commissionHelper,
            IFinanCityIdentity identity,
            Chatting chatting)
        {
            _overridingProfileItem = overridingProfileItem;
            _notification = notification;
            _careerManagement = careerManagement;
            _accountingPeriod = accountingPeriod;
            _marketingExpense = marketingExpense;
            _commissionPayment = commissionPayment;
            _commSchemeDetail = commSchemeDetail;
            _employment = employment;
            _allocation = allocation;
            _person = person;
            _commissionHelper = commissionHelper;
            _kendoGridFilter = kendoGridFilter;
            _contact = contact;
            _chatting = chatting;
            _identity = identity;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("getGrossToNextPromotion/{key}")]
        public decimal GetGrossToNextPromotion(string key)
        {
            try
            {
                return _commissionHelper.FindGrossToNextPromote(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getCloseSalesCommissionToDate/{key}")]
        public decimal GetCloseSalesCommissionToDate(string key)
        {
            try
            {
                return _commissionHelper.GetSalesCommissionToDate(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getCloseSalesCommissionToYear/{key}")]
        public decimal GetCloseSalesCommissionToYear(string key)
        {
            try
            {
                return _commissionHelper.GetSalesCommissionToYear(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getCloseSalesValueToDate/{key}")]
        public decimal GetCloseSalesValueToDate(string key)
        {
            try
            {
                return _commissionHelper.GetSalesValueToDate(spsk: key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getCloseSalesValueToYear/{key}")]
        public decimal GetCloseSalesValueToYear(string key)
        {
            try
            {
                return _commissionHelper.GetSalesValueToYear(spsk: key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getSalespersonUpTree/{key}")]
        public IEnumerable<SalespersonRecursiveTree> GetSalespersonUpTree(string key)
        {
            try
            {

                return _person.GetSalespersonRecursiveTree(key: key,
                   source: _person.GetSalepersonTire(),
                   downTree: false,
                   reverse: true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getSalespersonDownTree/{key}")]
        public IEnumerable<SalespersonRecursiveTree> GetSalespersonDownTree(string key)
        {
            try
            {
                return _person.GetSalespersonRecursiveTree(key: key,
                    source: _person.GetSalepersonTire(),
                    downTree: true,
                    reverse: false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getSalespersonFullTree/{key}")]
        public IEnumerable<SalespersonRecursiveTree> GetSalespersonFullTree(string key)
        {
            try
            {
                var source = _person.GetSalepersonTire();
                var person = source.Where(x => x.SPSK == key).SingleOrDefault();

                if (person != null)
                {
                    var group = person.GroupNum;
                    var level = source.Where(x => x.GroupNum == group).Min(x => x.Level);
                    var top = source.Where(x => x.GroupNum == group && x.Level == level).Single();

                    var result = _person.GetSalespersonRecursiveTree(key: top.SPSK,
                        source: source,
                        downTree: true,
                        reverse: false);

                    return result;
                }

                return new List<SalespersonRecursiveTree>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getCareerManagementDownTree/{key}")]
        public IEnumerable<SalespersonRecursiveTree> GetCareerManagementDownTree(string key)
        {
            try
            {
                var c = _careerManagement.Finds(filter: f => f.SysKey == key).Single();
                var s = _careerManagement.GetCareerManagementDownTree(key: key);

                return _person.GetSalespersonRecursiveTree(key: c.T1,
                   source: s,
                   downTree: true,
                   reverse: false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getCareerManagementFullTree/{key}")]
        public IEnumerable<SalespersonRecursiveTree> GetCareerManagementFullTree(string key)
        {
            try
            {
                var c = _careerManagement.Finds(filter: f => f.SysKey == key).Single();
                var s = _careerManagement.GetCareerManagementDownTree(key: key);
                var k = s.Where(x => x.SPSK == c.SPSK).LastOrDefault();
                var l = s.Where(x => x.GroupNum == k.GroupNum).Min(x => x.Level);
                var j = s.Where(x => x.GroupNum == k.GroupNum && x.Level == l).SingleOrDefault();

                return _person.GetSalespersonRecursiveTree(key: j.SPSK,
                   source: s,
                   downTree: true,
                   reverse: false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getCurrentCommissionSchemeByKey/{key}")]
        public CommissionStructureDetailEntity GetCommSchemeByKey(string key)
        {
            try
            {
                var emp = _employment.Finds(filter: f => f.SysKey == key).SingleOrDefault();
                return _commSchemeDetail.Finds(filter: f => f.SysKey == emp.SchemeDetailSysKey).SingleOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getCommissionSchemeByKey/{key}")]
        public IEnumerable<CommissionStructureDetailEntity> GetCommSchemeDetail(string key)
        {
            try
            {
                var emp = _employment.Finds(filter: f => f.SysKey == key).SingleOrDefault();
                var parentsk = _commSchemeDetail.Finds(filter: f => f.SysKey == emp.SchemeDetailSysKey).SingleOrDefault().ParentSK;

                return _commSchemeDetail.Finds(filter: f => f.ParentSK == parentsk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getEveryBody")]
        public IEnumerable<PersonEntity> GetEveryBody()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PersonEntity>(h);

                return _person.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("SalespersonTotalReceivable/{spsk}/{invoicesk}")]
        public decimal SalespersonTotalReceivable(string spsk, string invoicesk)
        {
            try
            {
                return _allocation.SalespersonTotalReceivable(spsk: spsk,
                    invoicesk: invoicesk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("SalespersonTotalReceived/{spsk}/{invoicesk}")]
        public decimal SalespersonTotalReceived(string spsk, string invoicesk)
        {
            try
            {
                return _allocation.SalespersonTotalReceived(spsk: spsk, invoicesk: invoicesk);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetCommissionPayableDetailPager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CommissionPayableDetailDTO> GetCommissionPayableDetailPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionPayableDetailDTO>(h);

                return _commissionPayment.GetDetailPager(currentPage: currentPage, pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetMarketingExpense/{spsk}")]
        public decimal GetMarketingExpense(string spsk)
        {
            try
            {
                return _marketingExpense.GetMarketingExpense(spsk);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getAccountingPeriodDTO/{yc}")]
        public IEnumerable<AccountingPeriodDTO> GetAccountingPeriod(string yc = "00")
        {
            try
            {
                return _accountingPeriod.GetAccountingPeriodDTO(yc);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getLastAccountingYearCode")]
        public string LastAccountingYearCode()
        {
            try
            {
                return _accountingPeriod.LastAccountingYearCode();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getLastAccountingYear")]
        public int? LastAccountingYear()
        {
            try
            {
                return _accountingPeriod.LastAccountingYear();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getCurrentAccountingYearCode")]
        public string CurrentAccountingYearCode()
        {
            try
            {
                return _accountingPeriod.CurrentAccountingYearCode();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getCurrentAccountingYear")]
        public int? CurrentAccountingYear()
        {
            try
            {
                return _accountingPeriod.CurrentAccountingYear();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("isTierCommission/{spsk}")]
        public bool IsTierCommission(string spsk)
        {
            try
            {
                return _person.IsTierCommission(spsk: spsk);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("schemeHeaderKey/{spsk}")]
        public string SchemeHeaderKey(string spsk)
        {
            var d = _employment.Finds(filter: f => f.SysKey == spsk).Single()
                .SchemeDetailSysKey;

            var p = _commSchemeDetail.Finds(filter: f => f.SysKey == d).Single().ParentSK;

            return p;
        }

        [HttpGet]
        [Route("pmt/{rate}/{months}/{loan}")]
        public decimal GetLoanPaymentAmount(double rate, double months, double loan)
        {
            var result = (decimal)(Microsoft.VisualBasic.Financial.Pmt((rate / 100) / 12, months * 12, loan, 0, Microsoft.VisualBasic.DueDate.EndOfPeriod) * -1);

            return result;
        }

        [HttpGet]
        [Route("address/{key}")]
        public string FullAddress(string key)
        {
            try
            {
                return _contact.FullAddress(key);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("senior/{spsk}/{rank}")]
        public string GetSenior(string spsk, RankType rank)
        {
            try
            {
                return _person.GetSenior(spsk, rank)?.Name;

                //_overridingProfileItem.GetSenior()
                //var members = _recursiveSalesBonusBuilder
                //    .Create(spsk: spsk, transactionDate: GeneralUtility.ServerDate)
                //    .GetSalesTeam();
                //if (members.Where(x => x.Key == rank).Count() > 0)
                //{
                //    var s = members[rank].SPSK;
                //    return _person.Finds(filter: f => f.SysKey == s).SingleOrDefault()?.Name;
                //}
                //return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("lastRemark/{leadsk}")]
        public ChattingEntity LastRemark(string leadsk)
        {
            try
            {
                return _chatting.Finds(filter: f => f.RefSK == leadsk)
                       .LastOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}