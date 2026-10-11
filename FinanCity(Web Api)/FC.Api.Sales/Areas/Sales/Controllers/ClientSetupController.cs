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
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using Ninject;

    [RoutePrefix("Client")]
    public class ClientSetupController : BaseApiController
    {
        #region Fields

        Person _person;
        Contact _contact;
        KendoGridFilterTransalator _kendoGridFilter;
        ClientSetupValidator _validator;

        #endregion

        #region Ctor

        public ClientSetupController(Person person,
            Contact contact,
            ClientSetupValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
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

                return _person.GetClientPager(currentPage: currentPage,
                    pageSize: pageSize,
                    where: e);
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

                return _person.GetClientAndLead(where: e);
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
                var client = _person.Finds(filter: f => f.SysKey == key).SingleOrDefault() ?? new PersonEntity();
                var contact = _contact.Finds(filter: f => f.SysKey == key).SingleOrDefault() ?? new ContactEntity();

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
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.Person.SysKey = _person.CreateAsClientTransaction(person: model.Person, 
                        contact: model.Contact);
                    
                    scope.Complete();
                }
                return Ok(new PersonAndContactModel
                {
                    Contact = _contact.Finds(filter: f => f.SysKey == model.Person.SysKey).Single(),
                    Person = _person.Finds(filter: f => f.SysKey == model.Person.SysKey).Single()
                });
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
                _validator.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _person.UpdateAsClientTransaction(person: model.Person,
                        contact: model.Contact);

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
                    _person.DeleteAsClientTransaction(syskey: syskey);

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
            var di = DI.Current.Get<IEssAppRepository>();
            return new PersonAndContactModel
            {
                Person = new PersonEntity
                {
                    SysKey = GeneralUtility.Key,
                    TransType = (int)TransType.Client,
                    Salutation = "MR",
                    Gender = (int)Gender.Male,
                    MartialStatus = (int)MartialStatus.Married,
                    IsOrganization = false,
                    IsActive = true,
                    IsLastNameFirst = true,
                    Name = null,
                    FirstName = null,
                    LastName = null,
                },
                Contact = new ContactEntity { Country = di.DefaultCountry }
            };
        }

        #endregion
    }
}
