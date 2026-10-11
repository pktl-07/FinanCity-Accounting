namespace FC.Domain.Version.DAO
{
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.1.0.11", Description = @"")]
	public class V_1_1_0_11_DAO : DAO<VersionEntity>, IEssVersion
	{
		#region Ctor

		public V_1_1_0_11_DAO()
		{
		}

		#endregion

		#region Methods

		public void Excute()
		{
			DbExcutor.ExcuteNonQuery(@"
			UPDATE EMP_History SET DateJoin = '20200901' WHERE DateJoin IS NULL

			UPDATE c SET  c.EffectiveDate = h.DateJoin 
			FROM CareerManagement [c]
			INNER JOIN EMP_History [h] ON [h].SysKey = c.SPSK

			", CommonConstant.SalesConnection);
		}

		#endregion
	}
}

