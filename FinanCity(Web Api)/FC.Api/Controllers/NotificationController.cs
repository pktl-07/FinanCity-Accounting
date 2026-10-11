namespace FC.Api.Controllers
{
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Core.SignalR;
    using FC.Domain;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Web;
    using System.Web.Http;

    [RoutePrefix("Notification")]
    public class NotificationController : BaseApiController
    {
        #region Fields

        private Notification _notification;
        private IFinanCityIdentity _identity;
        private KendoGridFilterTransalator _kendoGridFilter;

        #endregion

        #region Ctor

        public NotificationController(Notification notification,
            KendoGridFilterTransalator kendoGridFilter,
            IFinanCityIdentity identity)
        {
            _notification = notification;
            _identity = identity;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        //[HttpGet]
        //[Route("subscribe")]
        //public void Subscribe()
        //{
        //    var referral = DI.Current.Get<ReferralSalesNotificationService>();
        //    referral.Register();
        //    referral.OnDbChange += Referral_OnDbChange;
        //}

        [HttpGet]
        [Route("getChattingCount")]
        public int GetChattingCount()
        {
            try
            {
                var remarks = DI.Current.Get<RemarkNotificationService>();
                remarks.OnDbChange += Remarks_OnDbChange;

                var key = _identity.SPSK;
                var counter = _notification.Finds(filter: f => f.ReceiverSK == key
                 && f.IsRead == false
                 && f.TransType == (int)TransType.Chatting).Count();

                return counter;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getChatting")]
        public IEnumerable<NotificationEntity> GetChatting()
        {
            try
            {
                var key = _identity.SPSK;

                return _notification.Finds(filter: f => f.ReceiverSK == key
                 && f.IsRead == false
                 && f.TransType == (int)TransType.Chatting);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getMortgageSalesCount")]
        public int GetMortgageSalesCount()
        {
            try
            {
                var mortgage = DI.Current.Get<SalesMortgageNotificationService>();
                mortgage.OnDbChange += Mortgage_OnDbChange;
              
                var key = _identity.SPSK;
                var counter = _notification.Finds(filter: f => f.ReceiverSK == key
                 && f.IsRead == false
                 && f.TransType == (int)TransType.PropertyTransaction)
                    .Count();

                return counter;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getLeadFollowUpCount")]
        public int GetLeadFollowUpCount()
        {
            var followup = DI.Current.Get<LeadFollowUpNotificationService>();
            followup.OnDbChange += Followup_OnDbChange;

            var key = _identity.SPSK;
            var counter = _notification.GetLeadFollowUpDTO(filter: f => f.ReceiverSK == key
            && f.IsRead == false)
                .Count();

            return counter;
        }

        [HttpGet]
        [Route("getLeadFollowUp")]
        public IEnumerable<LeadFollowUpDTO> GetLeadFollowUp()
        {
            try
            {
                var key = _identity.SPSK;
                return _notification.GetLeadFollowUpDTO(filter: f => f.ReceiverSK == key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getMortgageSales")]
        public IEnumerable<NotificatonMortageSalesDTO> GetMortgageSales()
        {
            try
            {
                var key = _identity.SPSK;

                return _notification.GetNotificationMortgageSales(filter: f => f.ReceiverSK == key
                 && f.IsRead == false
                 && f.TransType == (int)TransType.PropertyTransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPut]
        [Route("hide/{key}")]
        public IHttpActionResult HideMessage(string key)
        {
            try
            {
                var data = _notification.Finds(filter: f => f.SysKey == key).Single();

                _notification.Update(selector: (s) => new NotificationEntity
                {
                    RecordStatus = (int)RecordStatus.Update,
                    IsRead = true,
                }, filter: f => f.SysKey == key);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("mortgagePager/{currentPage}/{pageSize}")]
        public IDataPagerResult<NotificatonMortageSalesDTO> GetMortgagePager(
            int currentPage = 0,
            int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<NotificatonMortageSalesDTO>(h);
                var key = _identity.SPSK;
                
                e = e.Constant(x => x.ReceiverSK == key
                && x.IsRead == false
                && x.TransType == (int)TransType.PropertyTransaction);

                return _notification.GetMortgagePager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("chattingPager/{currentPage}/{pageSize}")]
        public IDataPagerResult<NotificationEntity> GetChattingPager(
            int currentPage = 0,
            int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<NotificationEntity>(h);
                var key = _identity.SPSK;

                e = e.Constant(x => x.ReceiverSK == key
                && x.IsRead == false
                && x.TransType == (int)TransType.Chatting);

                return _notification.GetPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("followUpPager/{currentPage}/{pageSize}")]
        public IDataPagerResult<LeadFollowUpDTO> GetFollowUpPager(
            int currentPage = 0,
            int pageSize = 0)
        {
            try
            {
                var h = HttpContext.Current.Request.Headers["expression"];
                var e = _kendoGridFilter.Convert<LeadFollowUpDTO>(h);
                var key = _identity.SPSK;

                e = e.Constant(x => x.ReceiverSK == key &&
                x.IsRead == false);

                return _notification.GetLeadFollowUpPager(currentPage, pageSize, e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("getAndriodVersion")]
        public string GetAndriodVersion()
        {
            try
            {
               return VersionFileWatcher.Version.Android;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getIOSVersion")]
        public string GetIOSVersion()
        {
            try
            {
                return VersionFileWatcher.Version.IOS;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpDelete]
        [AllowAnonymous]
        [Route("wipeAll/{spsk}")]
        public IHttpActionResult ForceNotification(string spsk)
        {
            try
            {
                _notification.ForceNotify(spsk: spsk,
                transType: TransType.PropertyTransaction);

                _notification.ForceNotify(spsk: spsk,
               transType: TransType.Chatting);

                _notification.ForceNotify(spsk: spsk,
                transType: TransType.ReferralSalesOrder);

                _notification.ForceNotify(spsk: spsk,
                transType: TransType.LeadFollowUp);

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utilities

        private void Remarks_OnDbChange(object sender, SqlNotificationEventArgs e)
        {
            var s = sender as SqlDependency;
            if (s != null)
                s.OnChange -= Remarks_OnDbChange;

            FCHub.NotifyOnChattingChange();
        }

        private void Followup_OnDbChange(object sender, SqlNotificationEventArgs e)
        {
            var s = sender as SqlDependency;
            if (s != null)
                s.OnChange -= Followup_OnDbChange;

            FCHub.NotifyOnLeadFollowUp();
        }

        private void Mortgage_OnDbChange(object sender, SqlNotificationEventArgs e)
        {
            var s = sender as SqlDependency;
            if (s != null)
            {
                s.OnChange -= Mortgage_OnDbChange;
                FCHub.NotifyOnSalesMortgageChange();
            }
        }

        private void Referral_OnDbChange(object sender, SqlNotificationEventArgs e)
        {
            var s = sender as SqlDependency;
            if (s != null)
                s.OnChange -= Referral_OnDbChange;

            FCHub.NotifyOnReferralSalesChange();
        }

        #endregion
    }
}

