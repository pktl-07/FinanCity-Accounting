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
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Api.SalesAreas.Areas.Validators;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using Ninject;

    [RoutePrefix("Vendor")]
    public class VendorController : BaseApiController
    {
        #region Fields

        Company _company;
        Contact _contact;
        KendoGridFilterTransalator _kendoGridFilter;
        VendorSetupValidator _validator;
        Person _person;

        #endregion

        #region Ctor

        public VendorController(Company company, 
            KendoGridFilterTransalator kendoGridFilter,
            VendorSetupValidator validator,
            Person person,
            Contact contact)
        {
            _person = person;
            _company = company;
            _contact = contact;
            _kendoGridFilter = kendoGridFilter;
            _validator = validator;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<CompanyAndContactDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CompanyAndContactDTO>(h);
                e = e.Constant((x) => x.SysKey != "1" && x.TransType != (int)TransType.Franchisee);

                return _company.GetPager(currentPage, pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<CompanyEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<CompanyEntity>(h);

                return _company.Finds(filter: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getDefault")]
        public CompanyAndContactModel Default()
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

        [HttpGet]
        [Route("getByKey/{key}")]
        public CompanyAndContactModel Find(string key)
        {
            try
            {
                return new CompanyAndContactModel
                {
                    Vendor = _company.Finds(filter: f => f.SysKey == key).SingleOrDefault(),
                    Contact = _contact.Finds(filter: f => f.SysKey == key).SingleOrDefault()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(CompanyAndContactModel model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    model.Vendor = _company.Insert(model.Vendor);

                    //_person.Insert(new PersonEntity
                    //{
                    //    Name = "COMPANY REPRESENTS (DEFAULT)",
                    //    IsLastNameFirst = false,
                    //    Salutation = "MR",
                    //    Gender = 1,
                    //    TransType = (int)TransType.ExternalSalesPerson,
                    //    VendorSK = model.Vendor.SysKey,
                    //    IsDefault = true,
                    //    NRIC = model.Vendor.SysKey,
                    //});

                    model.Contact.SysKey = model.Vendor.SysKey; // add releation key
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
        public IHttpActionResult Update(CompanyAndContactModel model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _validator.ValidateAndThrow(model);

                    _company.Update(model.Vendor, filter: f => f.SysKey == model.Vendor.SysKey);
                    _contact.Update(model.Contact, filter: f => f.SysKey == model.Contact.SysKey);

                    scope.Complete();
                }
                return Ok(new CompanyAndContactModel
                {
                    Vendor = _company.Finds(f => f.SysKey == model.Vendor.SysKey).Single(),
                    Contact = _contact.Finds(f => f.SysKey == model.Contact.SysKey).Single(),
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
                    _company.Delete(filter: f => f.SysKey == syskey);
                    _contact.Delete(filter: f => f.SysKey == syskey);

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

        private CompanyAndContactModel GetDefaultModel()
        {
            var repo = DI.Current.Get<IEssAppRepository>();
            var defaultTax = repo.DefaultPurchaseZeroTax.TaxSK;

            return new CompanyAndContactModel
            {
                Vendor = new CompanyEntity
                {
                    SysKey = GeneralUtility.Key,
                    TaxSK = defaultTax,
                    TransType = (int)TransType.RealEstateAgent,
                    IsActive = true
                },
                Contact = new ContactEntity { Country = repo.DefaultCountry }
            };
        }

        #endregion
    }
}
