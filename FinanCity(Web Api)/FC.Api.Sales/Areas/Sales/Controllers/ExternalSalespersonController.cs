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

    [RoutePrefix("ExternalSalesperson")]
    public class ExternalSalespersonController : BaseApiController
    {
        #region Fields

        Person _person;
        Contact _contact;
        KendoGridFilterTransalator _kendoGridFilter;
        ExternalSalespersonValidator _validator;
        IEssAppRepository _app;

        #endregion

        #region Ctor

        public ExternalSalespersonController(Person person,
            Contact contact,
            IEssAppRepository app,
            ExternalSalespersonValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _app = app;
            _person = person;
            _contact = contact;
            _kendoGridFilter = kendoGridFilter;
            _validator = validator;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<LeadAndContactDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<LeadAndContactDTO>(h);

                //  e = e.Constance(x => x.TransType == (int)TransType.ExternalSalesPerson && x.Name != "COMPANY REPRESENTS (DEFAULT)");
                e = e.Constant((x) => x.TransType == (int)TransType.ExternalSalesPerson);
                
                return _person.GetExternalSalesperson(currentPage: currentPage, pageSize: pageSize, where: e);
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

                e = e.Constant((x) => x.TransType == (int)TransType.ExternalSalesPerson);

                return _person.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public PersonAndContactModel Find(string key)
        {
            try
            {
                var client = _person.Finds(filter: f => f.SysKey == key).SingleOrDefault();
                var contact = _contact.Finds(filter: f => f.SysKey == key).SingleOrDefault();
             
                return new PersonAndContactModel
                {
                    Person = client,
                    Contact = contact
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public PersonAndContactModel Default()
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
        public IHttpActionResult Save(PersonAndContactModel model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    model.Person = _person.Insert(model.Person);
                    model.Contact.SysKey = model.Person.SysKey;
                    model.Contact = _contact.Insert(model.Contact);

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
        public IHttpActionResult Update(PersonAndContactModel model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    _person.Update(entity: model.Person, filter: f => f.SysKey == model.Person.SysKey);
                    _contact.Update(entity: model.Contact, filter: f => f.SysKey == model.Contact.SysKey);

                    scope.Complete();

                }

                return Ok(new PersonAndContactModel
                {
                    Person = _person.Finds(x => x.SysKey == model.Person.SysKey).Single(),
                    Contact = _contact.Finds(x => x.SysKey == model.Contact.SysKey).Single()
                });
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
                    _contact.Delete(filter: f => f.SysKey == syskey);
                    _person.Delete(filter: f => f.SysKey == syskey);

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

        private PersonAndContactModel GetDefaultModel()
        {
            return new PersonAndContactModel
            {
                Person = new PersonEntity
                {
                    IsLastNameFirst = true,
                    TransType = (int)TransType.ExternalSalesPerson,
                    Salutation = "MR",
                    Gender = (int)Gender.Male,
                    MartialStatus = (int)MartialStatus.Married,
                    IsOrganization = false,
                    IsActive = true
                },
                Contact = new ContactEntity
                {
                    Country = _app.DefaultCountry,
                }
            };
        }

        #endregion
    }
}
