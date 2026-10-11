namespace FC.Domain.Accounts.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core.Data;
    using FC.Core.Infrastructure;
    using FC.Domain.Accounts.DAO;
    using FC.Domain.Accounts.Entity;

    public class BatchControl : BusinessLogic<BatchControlEntity, BatchControlDAO>
    {
        #region Methods

        public void CreateBTable(string tableName)
        {
            try
            {
                Guard.IsNotNullOrEmpty(tableName, "tableName");

                DAO.CreateBTable(tableName);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetBatchTableName(string yearCode, string moduleCode)
        {
            try
            {
                Guard.IsNotNullOrEmpty(yearCode, "yearCode");
                Guard.IsNotNullOrEmpty(moduleCode, "moduleCode");

                return Finds(filter: f => f.YC == yearCode && f.ModuleCode == moduleCode).Single().BatchTableName;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public BatchControlEntity MaxBatchTable()
        {
            try
            {
                return DAO.MaxBatchTable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<string> GetBTableByDateRange(
            int fromDate,
            int toDate,
            string moduleCode = null)
        {
            try
            {
                return DAO.GetBTableByDateRange(
                    fromDate: fromDate,
                    toDate: toDate,
                    moduleCode: moduleCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
