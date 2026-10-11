namespace FC.Domain.Version.DAO
{
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Sales.Business;
    using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.0.0.6", Description = @"")]
	public class V_1_0_0_6_DAO : DAO<VersionEntity>, IEssVersion
	{
		#region Fields

		private Person _person;
		private FollowUp _followup;
		private Notification _notification;

		#endregion

		#region Ctor

		public V_1_0_0_6_DAO(Person person,
		   FollowUp followup,
		   Notification notification)
		{
			_person = person;
			_followup = followup;
			_notification = notification;
		}

		#endregion

		#region Methods

		public void Excute()
		{
			DbExcutor.ExcuteNonQuery(@"ALTER TABLE Lead ADD LeadCloseRate INT",
				CommonConstant.SalesConnection);

			_person.Finds(filter: f => f.TransType == (int)TransType.Lead)
			.Each(person =>
			{
				_followup.Insert(new Sales.Entity.FollowUpEntity
				{
					LoginUser = "phyokayw.tl@gmail.com",
					Notify = false,
					NotifyMeBeforeStartDate = 1,
					RefSK = person.SysKey,
					StartDate = 20201230,
					StartTime = "1230",
				});
			});

			_followup.Finds().Each(f =>
			{
				_notification.Insert(new Sales.Entity.NotificationEntity
				{
					LoginUser = "phyokayw.tl@gmail.com",
					IsRead = false,
					RefSK = f.SysKey,
					SenderSK = f.RefSK,
					ReceiverSK = f.RefSK,
					TransType = (int)TransType.LeadFollowUp,
				});
			});

			DbExcutor.ExcuteNonQuery(@"UPDATE Lead SET LeadCloseRate = 1",
				CommonConstant.SalesConnection);
		}

		#endregion
	}
}
