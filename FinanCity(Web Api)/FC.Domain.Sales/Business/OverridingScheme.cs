namespace FC.Domain.Sales.Business
{
    using System.Collections.Generic;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;

    public class OverridingScheme : BusinessLogic<OverridingSchemeEntity, OverridingSchemeDAO>
    {
        #region Methods

        public IEnumerable<OverridingSchemeEntity> FindLastBatch()
        {
            try
            {
                return DAO.FindLastBatch();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
