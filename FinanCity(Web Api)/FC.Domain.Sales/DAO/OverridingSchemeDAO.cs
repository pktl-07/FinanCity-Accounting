namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Collections.Generic;
    using FC.Core.Data;
    using FC.Domain.Sales.Entity;
   
    public class OverridingSchemeDAO : DAO<OverridingSchemeEntity>
    {
        #region Methods

        public IEnumerable<OverridingSchemeEntity> FindLastBatch()
        {
            try
            {
                var query = $@"SELECT * FROM {Table<OverridingSchemeEntity>()}
                     WHERE {With<OverridingSchemeEntity>(x => x.BatchNo)}
                    =( SELECT {With<OverridingSchemeEntity>(x => x.BatchNo)} FROM {Table<OverridingSchemeEntity>()}
                    WHERE {With<OverridingSchemeEntity>(x => x.Auto)}
                    =(
                    SELECT MAX ({With<OverridingSchemeEntity>(x => x.Auto)}) FROM {Table<OverridingSchemeEntity>()}
                    )) ORDER BY {With<OverridingSchemeEntity>(x => x.MinCom)}";

                return DbExcutor<OverridingSchemeEntity>.ExcuteReader(query);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

    }
}
