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
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("SalespersonManagement")]
    public class SalespersonManagementController : BaseApiController
    {
        #region Fields

        private Person _person;
        private Employment _employment;
        private Contact _contact;
        private EmploymentHistory _employmentHistory;
        private KendoGridFilterTransalator _kendoGridFilter;
        private PersonIndustry _industryExam;
        private PersonEducation _qualification;
        private SalespersonManagementValidator _validator;
        private PayNow _payNow;
        private IEssAppRepository _app;

        #endregion

        #region Ctor

        public SalespersonManagementController(Person person,
            EmploymentHistory employmentHistory,
            SalespersonManagementValidator validator,
            Contact contact,
            PersonIndustry industryExam,
            PersonEducation qualification,
            Employment employment,
            PayNow payNow,
            IEssAppRepository app,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _app = app;
            _validator = validator;
            _qualification = qualification;
            _industryExam = industryExam;
            _employmentHistory = employmentHistory;
            _contact = contact;
            _employment = employment;
            _person = person;
            _kendoGridFilter = kendoGridFilter;
            _payNow = payNow;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<EmploymentDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<EmploymentDTO>(h);
                e = e.Constant((x) => x.TransType == (int)TransType.InternalSalesperson);

                return _person.GetEmploymentPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<PersonEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<PersonEntity>(h);
                e = e.Constant(x => x.TransType == (int)TransType.InternalSalesperson);

                return _person.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public SalespersonManagementModel Find(string key)
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
        public SalespersonManagementModel Default()
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
        public IHttpActionResult Save(SalespersonManagementModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _person.InsertAsSalespersonTransaction(person: model.Person,
                        contact: model.Contact,
                        employment: model.Employment,
                        employmentHistory: model.EmploymentHistory,
                        industry: model.Industry,
                        qualification: model.Qualification,
                        payNow: model.PayNow);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.Person.SysKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(SalespersonManagementModel model)
        {
            try
            {
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _person.UpdateAsSalespersonTransaction(person: model.Person,
                        contact: model.Contact,
                        employment: model.Employment,
                        employmentHistory: model.EmploymentHistory,
                        industry: model.Industry,
                        qualification: model.Qualification,
                        payNow: model.PayNow);

                    scope.Complete();
                }

                return Ok(GetDefaultModel(model.Person.SysKey));
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
                    _person.DeleteAsSalespersonTransaction(syskey);

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

        #region Utility

        private SalespersonManagementModel GetDefaultModel(string syskey = null)
        {
            if (string.IsNullOrWhiteSpace(syskey))
            {
                var key = GeneralUtility.Key;
                return new SalespersonManagementModel
                {
                    Contact = new ContactEntity
                    {
                        SysKey = key,
                        Country = _app.DefaultCountry,
                    },
                    Person = new PersonEntity
                    {
                        SysKey = key,
                        IsLastNameFirst = true,
                        TransType = (int)TransType.InternalSalesperson,
                        Salutation = "MR",
                        Gender = (int)Gender.Male,
                        VendorSK = "1",
                        MartialStatus = (int)MartialStatus.Married,
                        IsOrganization = false,
                        IsActive = true,
                        ReasonToTerminate = (int)SalepersonStatus.Live
                    },
                    Employment = new EmploymentEntity { SysKey = key, Rank = (int)RankType.SP },
                    EmploymentHistory = new EmploymentHistoryEntity { SysKey = key, YearsOfExperience = 0 },
                    Industry = new List<IndustryExamEntity>(),
                    Qualification = new List<QualificationEntity>(),
                    PayNow = new PayNowEntity(),
                };
            }
            else
            {
                return new SalespersonManagementModel
                {
                    Person = _person.Finds(filter: f => f.SysKey == syskey).SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == syskey).SingleOrDefault(),
                    Employment = _employment.Finds(filter: f => f.SysKey == syskey).SingleOrDefault(),
                    EmploymentHistory = _employmentHistory.Finds(filter: f => f.SysKey == syskey).SingleOrDefault(),
                    Industry = _person.GetIndustryExamByKey(syskey),
                    Qualification = _person.GetQualificationExamByKey(syskey),
                    PayNow = _payNow.Finds(filter: f => f.ParentSK == syskey).SingleOrDefault() ?? new PayNowEntity()
                };
            }
        }

        #endregion
    }
}
