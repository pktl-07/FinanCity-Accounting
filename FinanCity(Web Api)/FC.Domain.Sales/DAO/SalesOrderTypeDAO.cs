namespace FC.Domain.Sales.DAO
{
    using System;
    using System.Data.SqlClient;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using Ninject;

    public class SalesOrderTypeDAO : DAO<SalesOrderTypeEntity>
    {
        #region Methods

        public IDataPagerResult<SalesOrderTypeDTO> GetPager(int currentPage, int pageSize, Expression<Func<SalesOrderTypeDTO, bool>> filter = null)
        {
            try
            {
                var app = DI.Current.Get<IEssDataRepository>();
                var conn = app.DbConnection[CommonConstant.AccountConnection];
                var builder = new SqlConnectionStringBuilder(conn.ConnectionString);
                var db = builder.InitialCatalog;

                var query = $@"SELECT 
                    {With<SalesOrderTypeEntity>(x => x.SysKey)} [{nameof(SalesOrderTypeDTO.SysKey)}],
                    {With<SalesOrderTypeEntity>(x => x.Description)} [{nameof(SalesOrderTypeDTO.Description)}],
                    {With<SalesOrderTypeEntity>(x => x.IsActive)} [{nameof(SalesOrderTypeDTO.IsActive)}],
                    [acc].AccCode [{nameof(SalesOrderTypeDTO.AccountCode)}],
                    [acc].AccDescription [{nameof(SalesOrderTypeDTO.AccountDescription)}]
                    FROM 
                    {Table<SalesOrderTypeEntity>()}
                    INNER JOIN [{db}]..[ACC001] [acc] ON [acc].SysKey = {With<SalesOrderTypeEntity>(x => x.IncomeAccountSK)} ";

                query = $@"SELECT * FROM ({query}) [tbl] 
                           WHERE {filter.ToSQL()} ORDER BY 
                           [tbl].{nameof(SalesOrderTypeDTO.Description)}";

                return ExcutePagerResult<SalesOrderTypeDTO>(query, currentPage, pageSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
