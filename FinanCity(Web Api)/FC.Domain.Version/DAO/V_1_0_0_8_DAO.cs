namespace FC.Domain.Version.DAO
{
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Version.Entity;
    using FC.Domain.Version.Infrastructure;
  
    [ESSVersion(Name = "1.0.0.8", Description = @"")]
    public class V_1_0_0_8_DAO : DAO<VersionEntity>, IEssVersion
    {
   
        #region Ctor

        public V_1_0_0_8_DAO() { }

        #endregion

        #region Methods

        public void Excute()
        {
            DbExcutor.ExcuteNonQuery($@"
                ALTER TABLE AMS101 DROP COLUMN Debit
                ALTER TABLE AMS101 DROP COLUMN Credit
                DROP PROC DIS_001
                DROP PROC RPT_001
                DROP PROC RPT_001_2
                DROP PROC RPT_003_1
                DROP PROC RPT_008
                DROP PROC RPT_009    
                DROP PROC RPTDetail_001
                DROP PROC RPTHeader_001
                DROP PROC TAX_001
                DROP PROC UpdateStatusAll
            ", CommonConstant.SalesConnection);
        }

        #endregion
    }
}

