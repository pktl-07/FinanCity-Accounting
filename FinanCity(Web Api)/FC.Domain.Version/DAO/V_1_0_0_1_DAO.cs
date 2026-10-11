namespace FC.Domain.Version.DAO
{
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Version.Entity;
    using FC.Domain.Version.Infrastructure;
  
    [ESSVersion(Name = "1.0.0.1", Description = "Test")]
    public class V_1_0_0_1_DAO : DAO<VersionEntity>, IEssVersion
    {
        public V_1_0_0_1_DAO()
        {
        }

        public void Excute()
        {
            //do nothing. Test..
        }
    }
}
