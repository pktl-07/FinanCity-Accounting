namespace FC.Domain.Sales.Business
{
    using System.Linq;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;
    using Ninject;

    public class Chatting : BusinessLogic<ChattingEntity, ChattingDAO>
    {
        #region Fields

        private Notification _notification;
        private ClientShare _clientShare;
    
        private FinanCityIdentity _identity
        {
            get { return DI.Current.Get<FinanCityIdentity>(); }
        }

        #endregion

        #region Ctor

        public Chatting(Notification notification,
         ClientShare clientShare)
        {
            _clientShare = clientShare;
            _notification = notification;
        }

        #endregion

        #region Methods

        public ChattingEntity InsertAsTransaction(ChattingEntity entity)
        {
            try
            {
                entity = Insert(entity);
                var clients = _clientShare.Finds(filter: f => f.SysKey == entity.RefSK);

                if (_identity.AccountType == (int)UserAccountType.AdminAccount)
                {
                    clients.Where(x => x.RefSK != "1")
                        .Each(a =>
                    {
                        _notification.Insert(new NotificationEntity
                        {
                            SenderSK = "1",
                            ReceiverSK = a.RefSK,
                            RefSK = entity.RefSK,
                            Description = entity.Message,
                            TransType = (int)TransType.Chatting,
                        });
                    });
                }
                else if (_identity.AccountType == (int)UserAccountType.SalespersonAccount)
                {
                    clients.Where(x => x.RefSK != _identity.SPSK)
                        .Each(a =>
                    {
                        _notification.Insert(new NotificationEntity
                        {
                            SenderSK = _identity.SPSK,
                            ReceiverSK = a.RefSK,
                            RefSK = entity.RefSK,
                            Description = entity.Message,
                            TransType = (int)TransType.Chatting,
                        });
                    });
                }

                return entity;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
