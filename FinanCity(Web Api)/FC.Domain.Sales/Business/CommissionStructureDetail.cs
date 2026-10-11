namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using FC.Core.Data;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;

    public class CommissionStructureDetail : BusinessLogic<CommissionStructureDetailEntity, CommissionStructureDetailDAO>
    {
        public IEnumerable<CommissionStructureDetailEntity> FindBySPSK(string spsk)
        {
            try
            {
                return DAO.FindBySPSK(spsk);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
