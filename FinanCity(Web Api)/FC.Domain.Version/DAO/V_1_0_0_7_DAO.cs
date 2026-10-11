namespace FC.Domain.Version.DAO
{
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;
    using Ninject;

    [ESSVersion(Name = "1.0.0.7", Description = @"")]
	public class V_1_0_0_7_DAO : DAO<VersionEntity>, IEssVersion
	{
        #region Fields

        private TNC_Duration _duration
        {
            get { return DI.Current.Get<TNC_Duration>(); }
        }

        private SalesInvoice _salesInvoice
        {
            get { return DI.Current.Get<SalesInvoice>(); }
        }

        private PropertyTransaction _propertyTransaction
        {
            get { return DI.Current.Get<PropertyTransaction>(); }
        }

        private TNC_TimeFrame _timeFrame
        {
            get { return DI.Current.Get<TNC_TimeFrame>(); }
        }

		#endregion

		#region Ctor

		public V_1_0_0_7_DAO() { }

        #endregion

        #region Methods

        public void Excute()
        {
            _timeFrame.Insert(new TNC_TimeFrameEntity
            {
                IsActive = true,
                Description = "24-MONTHS"
            });

            _duration.Update(selector: s => new Sales.Entity.TNC_DurationEntity
            {
                RecordStatus = (int)RecordStatus.Update,
                Description = "2-years"
            }, filter: f => f.SysKey == "2017042514462609136");

            _duration.Update(selector: s => new Sales.Entity.TNC_DurationEntity
            {
                RecordStatus = (int)RecordStatus.Update,
                Description = "3-years"
            }, filter: f => f.SysKey == "2017042514462609137");

            _duration.Insert(new Sales.Entity.TNC_DurationEntity
            {
                Description = "4-years"
            });

            _duration.Insert(new Sales.Entity.TNC_DurationEntity
            {
                Description = "5-years"
            });

            DbExcutor.ExcuteNonQuery($@"
            UPDATE {Table<PropertyTransactionEntity>()}
            SET R1 = 'COM-100001', VoucherId = 100001 
            WHERE SysKey ='202011191031310038'",
            CommonConstant.SalesConnection);

            DbExcutor.ExcuteNonQuery($@"
            UPDATE {Table<SalesInvoiceEntity>()}
            SET R1 = 'INV-100001', VoucherId = 100001
            WHERE SysKey = '202011191031310038'
            ",CommonConstant.SalesConnection);
        }

		#endregion
	}
}

