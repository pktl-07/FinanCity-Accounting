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
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("CommissionScheme")]
    public class CommissionStructureController : BaseApiController
    {
        #region Fields

        private CommissionStructureBase _commissionStructureBase;
        private CommissionStructure _commissionScheme;
        private KendoGridFilterTransalator _kendoGridFilter;
        private CommissionSchemeValidator _validator;
        private CommissionStructureDetail _schemeDetail;
        private CommissionSchemeDeleteValidator _deleteValidator;
        
        #endregion

        #region Ctor

        public CommissionStructureController(CommissionStructure commissionScheme,
            CommissionStructureBase commissionStructureBase,
            CommissionStructureDetail schemeDetail,
            CommissionSchemeValidator validator,
            CommissionSchemeDeleteValidator deleteValidator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _commissionStructureBase = commissionStructureBase;
            _deleteValidator = deleteValidator;
            _commissionScheme = commissionScheme;
            _schemeDetail = schemeDetail;
            _kendoGridFilter = kendoGridFilter;
            _validator = validator;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CommissionStructureEntity> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionStructureEntity>(h);

                return _commissionScheme.GetPager(currentPage: currentPage, pageSize: pageSize, filter: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CommissionStructureEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CommissionStructureEntity>(h);

                return _commissionScheme.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public CommissionSchemeModel Find(string key)
        {
            try
            {
                return GetDefaultModel(key);
            }            
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CommissionSchemeModel Default()
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
        public IHttpActionResult Save(CommissionSchemeCrateOrUpdateModel model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    var a =  _commissionStructureBase.Finds(filter: f => f.SysKey == model.CommissionScheme.BaseHeaderSK).Single();

                    if (a.Type == (int)FC.Core.CommissonStructureType.Tier)
                    {
                        model.Inserted.Each(x =>
                        {
                            x.Type = (int)FC.Core.CommissonStructureType.Tier;
                        });
                    }

                    model.CommissionScheme.SysKey = _commissionScheme.BulkInsert(entity: model.CommissionScheme,
                        inserted: model.Inserted);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.CommissionScheme.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(CommissionSchemeCrateOrUpdateModel model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    _commissionScheme.BulkUpdate(entity: model.CommissionScheme,
                    inserted: model.Inserted,
                    deleted: model.Deleted,
                    updated: model.Updated);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.CommissionScheme.SysKey));
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
                _deleteValidator.ValidateAndThrow(syskey);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _commissionScheme.BulkDelete(commissionSchemeSK: syskey);

                    scope.Complete();
                }

                return Ok(GetDefaultModel());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utilities

        private CommissionSchemeModel GetDefaultModel(string syskey = null)
        {

            if (string.IsNullOrWhiteSpace(syskey))
            {
                return new CommissionSchemeModel
                {
                    CommissionScheme = new CommissionStructureEntity
                    {
                        IsActive = true,
                        BaseHeaderSK = "01",
                    },
                    CommissionSchemeDetail = new CommissionStructureDetailEntity[]
                    {
                        new CommissionStructureDetailEntity
                        {
                            Designation = CommissonStructureType.CompanyLeads.ToDescription(),
                            AgencyPercent = 50,
                            AssociatedPercent = 50,
                            BrifeDesignationName=CommissonStructureType.CompanyLeads.ToDescription(),
                            Type = (int)CommissonStructureType.CompanyLeads,
                            IsActive = true,
                        },
                        new CommissionStructureDetailEntity
                        {
                            Designation = CommissonStructureType.PersonalLeads.ToDescription(),
                            AgencyPercent = 30,
                            AssociatedPercent = 70,
                            BrifeDesignationName=CommissonStructureType.PersonalLeads.ToDescription(),
                            Type = (int)CommissonStructureType.PersonalLeads,
                            IsActive = true,
                        },
                        new CommissionStructureDetailEntity
                        {
                            Designation = CommissonStructureType.RESLeads.ToDescription(),
                            AgencyPercent = 50,
                            AssociatedPercent = 50,
                            BrifeDesignationName=CommissonStructureType.RESLeads.ToDescription(),
                            Type = (int)CommissonStructureType.RESLeads,
                            IsActive = true,
                        },
                    }
                };
            }
            else
            {
                return new CommissionSchemeModel
                {
                    CommissionScheme = _commissionScheme.Finds(filter: f => f.SysKey == syskey).Single(),
                    CommissionSchemeDetail = _schemeDetail.Finds(filter: f => f.ParentSK == syskey)
                };
            }
        }

        #endregion
    }
}
