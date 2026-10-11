namespace FC.Domain
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Configuration;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Sales.Entity;
    using FC.Core.Infrastructure.Interfaces;
    using Ninject;

    public delegate void DbChange(object sender, SqlNotificationEventArgs args);

    public class ReferralSalesNotificationService : BaseNotification
    {
        public ReferralSalesNotificationService() : base(TransType.ReferralSalesOrder) { }
    }

    public class SalesMortgageNotificationService : BaseNotification
    {
        public SalesMortgageNotificationService() : base(TransType.PropertyTransaction) { }
    }

    public class LeadFollowUpNotificationService : BaseNotification
    {
        public LeadFollowUpNotificationService() : base(TransType.LeadFollowUp) { }
    }

    public class RemarkNotificationService : BaseNotification
    {
        public RemarkNotificationService() : base(TransType.Chatting) { }
    }

    public class BaseNotification : DAO
    {
        #region Events

        public event DbChange OnDbChange;

        #endregion

        #region Fields

        private TransType _transType;
        private IFinanCityIdentity _identity = DI.Current.Get<IFinanCityIdentity>();

        #endregion

        #region Ctor

        public BaseNotification(TransType transType)
        {
            _transType = transType;
            RegisterQuery();
        }

        #endregion

        #region Methods

        private void RegisterQuery()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FC.BIZ"].ConnectionString);
            var command = new SqlCommand();

            // Clear any existing notifications
            command.Notification = null;
            command.CommandText = $@"SELECT 
                {With<NotificationEntity>(x => x.ReceiverSK)},
                {With<NotificationEntity>(x => x.IsRead)},
                {With<NotificationEntity>(x => x.TransType)}
                FROM dbo.{Table<NotificationEntity>()}
                WHERE {With<NotificationEntity>(x => x.TransType)} = {(int)_transType}
                AND {With<NotificationEntity>(x => x.ReceiverSK)} = '{_identity.SPSK}'";

            command.Connection = connection;
            command.CommandType = CommandType.Text;

            var sqlDependency = new SqlDependency(command);
            sqlDependency.OnChange += new OnChangeEventHandler((s, e) =>
            {
                this.OnDbChange?.Invoke(s, e);
            });

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }

        #endregion
    }
}
