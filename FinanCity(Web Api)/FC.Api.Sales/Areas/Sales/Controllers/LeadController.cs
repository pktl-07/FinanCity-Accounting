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
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Sales.DTO;
    using Ninject;

    [RoutePrefix("Lead")]
    public class LeadController : BaseApiController
    {
        #region Fields

        private Lead _lead;
        private KendoGridFilterTransalator _kendoGridFilter;
        private LeadValidation _leadValidation;
        private Person _person;
        private Contact _contact;
        private Chatting _chart;
        private ClientShare _clientShare;
        private FollowUp _followUp;
    
        #endregion

        #region Ctor

        public LeadController(Lead lead,
            Person person,
            Contact contact,
            FollowUp followup,
            Chatting chat,
            ClientShare clientShare,
            KendoGridFilterTransalator kendoGridFilter,
            LeadValidation leadValidation)
        {
            _followUp = followup;
            _chart = chat;
            _lead = lead;
            _person = person;
            _contact = contact;
            _clientShare = clientShare; 
            _kendoGridFilter = kendoGridFilter;
            _leadValidation = leadValidation;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("pager/{currentPage}/{pageSize}")]
        public IDataPagerResult<LeadManagementDTO> GetPager(int currentPage = 0, int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<LeadManagementDTO>(h);

                return _lead.GetPager(currentPage: currentPage, pageSize: pageSize, where: e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<LeadEntity> Finds()
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<LeadEntity>(h);
               
                return _lead.Finds(filter: e);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public LeadModel Find(string key)
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
        public LeadModel Default()
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
        public IHttpActionResult Save(LeadCreateModal model)
        {
            try
            {
                _leadValidation.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    model.Person.SysKey = _lead.InsertAsTransaction(person: model.Person,
                        contact: model.Contact,
                        lead: model.Lead,
                        followup: model.FollowUp);

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
        public IHttpActionResult Update(LeadUpdateModal model)
        {
            try
            {
                _leadValidation.ValidateAndThrow(model);

                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _lead.UpdateAsTransaction(person: model.Person,
                        contact: model.Contact,
                        lead: model.Lead,
                        followup : model.FollowUp,
                        insertedRemarks: model.InsertedRemarkItems,
                        deletedRemarks: model.DeletedRemarkItems,
                        insertedShareContacts: model.InsertedSharedContacts,
                        deletedShareContacts: model.DeletedSharedContacts);

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
                    _lead.DeleteAsTransaction(key: syskey);

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

        private LeadModel GetDefaultModel(string syskey = "")
        {
            var spsk = FinanCityIdentity.SPSK;
            var di = DI.Current.Get<IEssAppRepository>();

            if (string.IsNullOrWhiteSpace(syskey))
            {
                return new LeadModel
                {
                    Person = new PersonEntity
                    {
                        SysKey = GeneralUtility.Key,
                        TransType = (int)TransType.Lead,
                        Salutation = "MR",
                        Gender = (int)Gender.Male,
                        MartialStatus = (int)MartialStatus.Married,
                        IsOrganization = false,
                        IsActive = true,
                        IsLastNameFirst = true,
                    },
                    Lead = new LeadEntity { LeadCloseRate = 1 },
                    FollowUp = new FollowUpEntity {
                        Notify = true, 
                        NotifyMeBeforeStartDate = 0 
                    },
                    Contact = new ContactEntity
                    {
                        Country = di.DefaultCountry
                    },
                    Remarks = new ChattingEntity[] { },
                    ShareContacts = new string[] { },
                };
            }

            return new LeadModel
            {
                Person = _person.Finds(filter: f => f.SysKey == syskey).Single(),
                Contact = _contact.Finds(filter: f => f.SysKey == syskey).Single(),
                Lead = _lead.Finds(filter: f => f.SysKey == syskey).Single(),
                Remarks = _chart.Finds(filter: f => f.RefSK == syskey) ?? new ChattingEntity[] { },
                FollowUp = _followUp.Finds(filter: f => f.RefSK == syskey).SingleOrDefault() ?? new FollowUpEntity(),
                ShareContacts = _clientShare.Finds(f => f.SysKey == syskey)
                .Select((s) => s.RefSK)
                .Except(new string[] { spsk, "1" })
                .ToArray() ?? new string[] { },
            };
        }

        #endregion
    }
}
