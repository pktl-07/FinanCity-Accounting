namespace FC.Domain.Version.DAO
{
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.1.0.13", Description = @"")]
	public class V_1_1_0_13_DAO : DAO<VersionEntity>, IEssVersion
	{
		#region Ctor

		public V_1_1_0_13_DAO()
		{
		}

		#endregion

		#region Methods

		public void Excute()
		{
			DbExcutor.ExcuteNonQuery(@"
			UPDATE PayNow SET PayNow.MobileNumber = Contact.MobilePhone 
			FROM Contact INNER JOIN PayNow ON Contact.SysKey = PayNow.ParentSK 
			WHERE Contact.SysKey <> '202109011450140133'
			", CommonConstant.SalesConnection);
		}

		#endregion
	}
}

