namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using FC.Core.Data;
    using FC.Domain.Sales.Entity;

    public class CommissionStructureDetailDAO : DAO<CommissionStructureDetailEntity>
    {
        #region Methods

        public IEnumerable<CommissionStructureDetailEntity> FindBySPSK(string spsk)
        {
            var query = $@"SELECT * FROM {Table<CommissionStructureDetailEntity>()} 
                WHERE {With<CommissionStructureDetailEntity>(x => x.ParentSK)} =
                (SELECT {With<CommissionStructureDetailEntity>(x => x.ParentSK)}
                    FROM  {Table<CommissionStructureDetailEntity>()}
	                INNER JOIN {Table<EmploymentEntity>()} ON {With<EmploymentEntity>(x => x.SchemeDetailSysKey)} = {With<CommissionStructureDetailEntity>(x => x.SysKey)} 
	                WHERE {With<EmploymentEntity>(x => x.SysKey)} = <# {spsk} #> 
                )
                ORDER BY {With<CommissionStructureDetailEntity>(x => x.MinCom)}";

            try
            {
                return DbExcutor<CommissionStructureDetailEntity>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
