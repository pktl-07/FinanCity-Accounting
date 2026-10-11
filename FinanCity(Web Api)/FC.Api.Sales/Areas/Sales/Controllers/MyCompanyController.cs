namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using FC.Core;
    using FluentValidation;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Core;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Sales.Business;

    [RoutePrefix("MyCompany")]
    public class MyCompanyController : BaseApiController
    {
        #region Fields

        Company _company;
        Contact _contact;
        MyCompanyValidator _validator;
        KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public MyCompanyController(Company company,
            Contact contact,
            MyCompanyValidator validator,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _contact = contact;
            _validator = validator;
            _company = company;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("getByKey/{key}")]
        public CompanyAndContactModel Find(string key)
        {
            try
            {
                return new CompanyAndContactModel
                {
                    Vendor = _company.Finds(filter: f => f.SysKey == "1").SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == "1").SingleOrDefault()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(CompanyAndContactModel model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    _company.Update(entity: model.Vendor, filter: f => f.SysKey == model.Vendor.SysKey);
                    _contact.Update(entity: model.Contact, filter: f => f.SysKey == model.Contact.SysKey);

                    scope.Complete();
                }

                return Ok(_company.Finds(filter: f => f.SysKey == model.Vendor.SysKey).Single());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
