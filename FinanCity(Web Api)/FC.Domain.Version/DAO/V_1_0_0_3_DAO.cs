namespace FC.Domain.Version.DAO
{
    using FC.Core;
    using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.0.0.3", Description = "Addind file Aattachment.")]
	public class V_1_0_0_3_DAO : DAO<VersionEntity>, IEssVersion
	{
		public void Excute()
		{
			DbExcutor.ExcuteNonQuery(@"ALTER TABLE JUN012 ADD Type INT", 
				CommonConstant.SalesConnection);
			
			DbExcutor.ExcuteNonQuery(@"ALTER TABLE JUN012 ADD Size INT", 
				CommonConstant.SalesConnection);
			
			DbExcutor.ExcuteNonQuery(@"UPDATE JUN012 SET Type=1",
				CommonConstant.SalesConnection);
			
			DbExcutor.ExcuteNonQuery(@"UPDATE JUN012 SET Size=0",
				CommonConstant.SalesConnection);
		}
	}
}
