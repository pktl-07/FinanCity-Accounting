namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.Entity;
   
    public class CommissionStructureDAO : DAO<CommissionStructureEntity>
    {
        #region Methods

        public IDataPagerResult<CommissionStructureEntity> GetPager(int currentPage, int pageSize, Expression<Func<CommissionStructureEntity, bool>> where = null)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<CommissionStructureEntity>()}
                WHERE {where.ToSQL()}
                ORDER BY {With<CommissionStructureEntity>(x => x.Auto)} DESC";

                return ExcutePagerResult<CommissionStructureEntity>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public CommissionStructureEntity GetByDetailSK(string detailsk)
        {
            try
            {
                var query = $@"SELECT {Table<CommissionStructureEntity>()}.* FROM {Table<CommissionStructureEntity>()}
                INNER JOIN {Table<CommissionStructureDetailEntity>()} ON {With<CommissionStructureDetailEntity>(x => x.ParentSK)} = {With<CommissionStructureEntity>(x => x.SysKey)}
                WHERE {With<CommissionStructureDetailEntity>(x => x.SysKey)} = '{detailsk}'";

                return DbExcutor<CommissionStructureEntity>.ExcuteReader(query).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
