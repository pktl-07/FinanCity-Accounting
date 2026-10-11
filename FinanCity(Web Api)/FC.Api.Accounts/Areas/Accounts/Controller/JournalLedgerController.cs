namespace FC.Api.AccountAreas.Areas.Controllers
{
    using FC.Api.AccountAreas.Areas.Infrastructure.FluentValidators;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.Business;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
    using FluentValidation;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;

    [RoutePrefix("JournalLedger")]
    public class JournalLedgerController : BaseApiController
    {
        #region Fields

        BTable _journalLedger;
        KendoGridFilterTransalator _kendoGridFilter;
        AccountingPeriod _accountingPeriod;
        JournalLedgerValidator _validator;
        JournalLedgerReverseValidator _reverseVoidValidator;

        #endregion

        #region Ctor

        public JournalLedgerController(BTable journalLedger,
            AccountingPeriod accountingPeriod,
            JournalLedgerReverseValidator reverseVoidValidator,
            JournalLedgerValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _reverseVoidValidator = reverseVoidValidator;
            _validator = validator;
            _accountingPeriod = accountingPeriod;
            _journalLedger = journalLedger;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<JournalSummaryDTO> GetPager(int currentPage = 0, 
            int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<JournalSummaryDTO>(h);

                return _journalLedger.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<BTableEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<BTableEntity>(h);

                return _journalLedger.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(IEnumerable<BTableEntity> model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                var postDate = model.First().PostDate;
                var parentId = "";

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    parentId = _journalLedger.InsertAsTransaction(data: model,
                        moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                        postDate: postDate.Value,
                        transType: TransType.Journal);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(parentId: parentId,
                    moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                    yc: model.First().YCPC));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(IEnumerable<BTableEntity> model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                var postDate = model.First().PostDate;
                var parentId = model.First().ParentId;

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _journalLedger.UpdateAsTransaction(data: model,
                       moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                       parentId: parentId,
                       postDate: postDate.Value,
                       transType: TransType.Journal,
                       recordStatus:model.First().RecordStatus);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(parentId: parentId,
                    moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                    yc: model.First().YCPC));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("post")]
        public IHttpActionResult Post(IEnumerable<BTableEntity> model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                var postDate = model.First().PostDate;
                var parentId = model.First().ParentId;

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _journalLedger.PostAsTransaction(data: model,
                       moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                       parentId: parentId,
                       postDate: postDate.Value,
                       transType: TransType.Journal);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(parentId: parentId,
                    moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                    yc: model.First().YCPC));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("void/{parentId}/{postDate}")]
        public IHttpActionResult Void(string parentId, int postDate)
        {
            try
            {
                _reverseVoidValidator.ValidateAndThrow(_journalLedger.Finds(filter:
                    f => f.ParentId == parentId &&
                    f.PostDate == postDate &&
                    f.ModuleCode == CommonConstant.Module.MODULE_GENERAL_LEDGER));

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _journalLedger.VoidAsTransaction(
                       moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                       parentId: parentId,
                       postDate: postDate);

                    scope.Complete();
                }

                var currentYearCode = _accountingPeriod.CurrentAccountingYearCode();

                return Ok(GetDefaultModel(parentId: parentId,
                       moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                       yc: currentYearCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("reverse/{parentId}/{postDate}")]
        public IHttpActionResult Reverse(string parentId, int postDate)
        {
            try
            {
                _reverseVoidValidator.ValidateAndThrow(_journalLedger.Finds(filter:
                    f => f.ParentId == parentId &&
                    f.PostDate == postDate &&
                    f.ModuleCode == CommonConstant.Module.MODULE_GENERAL_LEDGER));

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _journalLedger.ReverseAsTransaction(
                       moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                       postDate: postDate,
                       parentId: parentId,
                       transType: TransType.Journal);

                    scope.Complete();
                }

                var currentYearCode = _accountingPeriod.CurrentAccountingYearCode();
                return Ok(GetDefaultModel(parentId: parentId,
                        moduleCode: CommonConstant.Module.MODULE_GENERAL_LEDGER,
                        yc: currentYearCode));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("recap/{parentId}/{moduleCode}/{postDate}")]
        public IEnumerable<BTableEntity> Recap(string parentId, string moduleCode, int? postDate)
        {
            try
            {
                return _journalLedger.Finds(filter: f =>
                     f.ParentId == parentId &&
                     f.ModuleCode == moduleCode &&
                     f.PostDate == postDate);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public JournalSummaryDTO Default()
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

        #endregion

        #region Utilities

        private JournalSummaryDTO GetDefaultModel(string parentId = null,
            string moduleCode = null,
            string yc = "")
        {
            if (!string.IsNullOrWhiteSpace(parentId) && !string.IsNullOrWhiteSpace(moduleCode))
            {
                return _journalLedger.GetSummary(filter: f => f.ParentId == parentId &&
                f.ModuleCode == moduleCode &&
                f.YC.StartsWith(yc)).Single();
            }
            
            var di = DI.Current.Get<IEssAppRepository>();
            return new JournalSummaryDTO
            {
                R1 = "TBA",
                CURRSK = di.CURRSK,
                CURRATE = di.CURRATE,
                CURROP = di.CURROP,
                PostDate = GeneralUtility.ServerDate,
            };
        }

        #endregion
    }
}
