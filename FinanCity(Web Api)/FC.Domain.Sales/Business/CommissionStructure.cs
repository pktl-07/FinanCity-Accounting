namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;
    
    public class CommissionStructure : BusinessLogic<CommissionStructureEntity, CommissionStructureDAO> , IDataPageable<CommissionStructureEntity>
    {
        #region Fields

        CommissionStructureDetail _promotionSchemeDetail;

        #endregion

        #region Ctor

        public CommissionStructure(CommissionStructureDetail promotionSchemeDetail)
        {
            _promotionSchemeDetail = promotionSchemeDetail;
        }

        #endregion
        
        #region Methods

        public IDataPagerResult<CommissionStructureEntity> GetPager(int currentPage, int pageSize, Expression<Func<CommissionStructureEntity, bool>> filter = null)
        {
            try
            {
                return DAO.GetPager(currentPage, pageSize, filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string BulkInsert(CommissionStructureEntity entity,
            IEnumerable<CommissionStructureDetailEntity> inserted)
        {
            try
            {
                entity = Insert(entity);

                inserted.Each(data =>
                {
                    data.ParentSK = entity.SysKey;
                    _promotionSchemeDetail.Insert(data);
                });

                return entity.SysKey;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void BulkUpdate(CommissionStructureEntity entity,
            IEnumerable<CommissionStructureDetailEntity> inserted,
            IEnumerable<CommissionStructureDetailEntity> updated,
            IEnumerable<CommissionStructureDetailEntity> deleted)
        {
            try
            {
                Update(entity: entity, filter: (f) => f.SysKey == entity.SysKey);

                inserted.Each(data =>
                {
                    data.ParentSK = entity.SysKey;
                    _promotionSchemeDetail.Insert(data);
                });

                updated.Each(data =>
                {
                    data.ParentSK = entity.SysKey;
                    _promotionSchemeDetail.Update(entity: data, filter: f => f.SysKey == data.SysKey);
                });

                deleted.Each(data =>
                {
                    _promotionSchemeDetail.Delete(filter: x => x.SysKey == data.SysKey);
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }    
        }

        public void BulkDelete(string commissionSchemeSK)
        {
            try
            {
                _promotionSchemeDetail.Finds(filter: f => f.ParentSK == commissionSchemeSK).Each(data =>
                {
                    _promotionSchemeDetail.Delete(filter: f => f.SysKey == data.SysKey);
                });

                Delete(filter: f => f.SysKey == commissionSchemeSK);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public CommissionStructureEntity GetByDetailSK(string empsk)
        {
            try
            {
                return DAO.GetByDetailSK(detailsk: empsk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
