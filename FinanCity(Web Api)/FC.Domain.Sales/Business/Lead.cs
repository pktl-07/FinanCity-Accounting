namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Core.Infrastructure.Implements.RecordIndexer;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    
    public class Lead : BusinessLogic<LeadEntity, LeadDAO>
    {
        #region Fields

        private Person _person;
        private Contact _contact;
        private RecordIndexerFactory _recordIndexer;
        private ClientShare _clientShare;
        private Chatting _chatting;
        private Notification _notification;
        private FollowUp _followup;

        #endregion

        #region Ctor

        public Lead(Person person,
            ClientShare clientShare,
            RecordIndexerFactory recordIndexer,
            Contact contact,
            FollowUp followup,
            Notification notification,
            Chatting chatting)
        {
            _followup = followup;
            _notification = notification;
            _recordIndexer = recordIndexer;
            _person = person;
            _contact = contact;
            _clientShare = clientShare;
            _chatting = chatting;
        }

        #endregion

        #region Methods

        public override IEnumerable<LeadEntity> Finds(Expression<Func<LeadEntity, bool>> filter)
        {
            try
            {
                return DAO.GetLead(filter: filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IDataPagerResult<LeadManagementDTO> GetPager(int currentPage, int pageSize, Expression<Func<LeadManagementDTO, bool>> where = null)
        {
            try
            {
                var refsk = FinanCityIdentity.SPSK;
                where = where.Constant(x => x.RefSK == refsk);

                return DAO.GetPager(currentPage, pageSize, where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string InsertAsTransaction(PersonEntity person,
            ContactEntity contact,
            LeadEntity lead,
            FollowUpEntity followup)
        {
            try
            {
                var index = _recordIndexer.Generate(TransType.Lead, lead);
                var refsk = FinanCityIdentity.SPSK;

                lead.R1 = index.R1;
                lead.Prefix = index.Prefix;
                lead.Postfix = index.Postfix;
                lead.VoucherId = index.VoucherId;

               followup.RefSK = contact.SysKey = lead.SysKey = _person.Insert(person).SysKey;

                Insert(lead);

                followup = _followup.Insert(followup);
               
                _contact.Insert(contact);

                if (FinanCityIdentity.AccountType == (int)UserAccountType.SalespersonAccount)
                    _clientShare.Insert(new ClientShareEntity { SysKey = lead.SysKey, RefSK = refsk });

                _clientShare.Insert(new ClientShareEntity { SysKey = lead.SysKey, RefSK = "1" });

                _notification.Insert(new NotificationEntity
                {
                    SenderSK = refsk,
                    ReceiverSK = refsk,
                    RefSK = followup.SysKey,
                    TransType = (int)TransType.LeadFollowUp,
                    Description =  followup.Description,
                });

                return lead.SysKey;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateAsTransaction(PersonEntity person,
            ContactEntity contact,
            LeadEntity lead,
            FollowUpEntity followup,
            IEnumerable<ChattingEntity> insertedRemarks,
            IEnumerable<ChattingEntity> deletedRemarks,
            IEnumerable<string> insertedShareContacts,
            IEnumerable<string> deletedShareContacts)
        {
            try
            {
                var loginsk = FinanCityIdentity.SPSK;

                _person.Update(person, filter: f => f.SysKey == person.SysKey);
                _contact.Update(contact, filter: f => f.SysKey == contact.SysKey);

                Update(lead, filter: f => f.SysKey == lead.SysKey);

                _followup.Update(followup, filter: f => f.SysKey == followup.SysKey);

                insertedRemarks?.Each(d =>
                {
                    d.RefSK = lead.SysKey;
                    _chatting.InsertAsTransaction(d);
                });

                deletedRemarks?.Each(d =>
                {
                    _chatting.Delete(filter: f => f.SysKey == d.SysKey);
                });

                insertedShareContacts?.Each((s) =>
                {
                    _notification.Insert(new NotificationEntity
                    {
                        ReceiverSK = s,
                        SenderSK = loginsk,
                        RefSK = followup.SysKey,
                        TransType = (int)TransType.LeadFollowUp,
                        Description = followup.Description,
                    });

                    _clientShare.Insert(new ClientShareEntity
                    {
                        SysKey = lead.SysKey,
                        RefSK = s
                    });
                });

                deletedShareContacts?.Each((s) =>
                {
                    _notification.Delete(filter:
                        f => f.RefSK == followup.SysKey
                     && f.SenderSK == loginsk
                     && f.ReceiverSK == s
                     && f.TransType == (int)TransType.LeadFollowUp);

                    _clientShare.Delete(f => f.SysKey == lead.SysKey
                    && f.RefSK == s);
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void DeleteAsTransaction(string key)
        {
            try
            {
                var refsk = FinanCityIdentity.SPSK;

                _person.Delete(filter: f => f.SysKey == key);
                _contact.Delete(filter: f => f.SysKey == key);

                _clientShare.Delete(filter: f => f.SysKey == key
                && f.RefSK == refsk);

                _followup.Delete(filter: f => f.RefSK == key);

                _notification.Delete(filter: f => f.SysKey == key);

                _clientShare.Delete(filter: f => f.SysKey == key
                && f.RefSK == "1");

                Delete(filter: f => f.SysKey == key);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void InActiveLeadWhoNeverBecomeClientFor(int day)
        {
            try
            {
                var leads = DAO.GetLeadWhoNeverBecomeClientFor(day: day);
                leads?.Each(l =>
                {
                    _person.Update(
                        selector: s => new PersonEntity
                        {
                            RecordStatus = (int)RecordStatus.Update,
                            IsActive = false
                        },
                        filter: f => f.SysKey == l);
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Memebers

        public IFinanCityIdentity FinanCityIdentity
        {
            get
            {
                return DI.Current.Get<IFinanCityIdentity>();
            }
        }

        #endregion
    }
}
