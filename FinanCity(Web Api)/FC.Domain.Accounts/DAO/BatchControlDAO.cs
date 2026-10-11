namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.Entity;

    public class BatchControlDAO : DAO<BatchControlEntity>
    {
        #region Methods

        public BatchControlEntity MaxBatchTable()
        {
            try
            {
                var query = $@" SELECT TOP 1 * FROM {Table<BatchControlEntity>()}
                            ORDER BY {With<BatchControlEntity>(x => x.BatchTableName)} DESC ";

                return DbExcutor<BatchControlEntity>.ExcuteReader(query).SingleOrDefault();

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
                var result = new List<string>();
                var startYC = (string)DbExcutor.ExcuteScalar($"SELECT YC1 FROM GetYCRange('{fromDate}','{toDate}')", "FC.Acc");
                var endYC = (string)DbExcutor.ExcuteScalar($"SELECT YC2 FROM GetYCRange('{fromDate}','{toDate}')", "FC.Acc");

                var query = $@"SELECT * FROM {Table<BatchControlEntity>()} WHERE YC IN ('{startYC}','{endYC}')";

                if (!string.IsNullOrEmpty(moduleCode))
                    query += $" AND {With<BatchControlEntity>(x => x.ModuleCode)}='{moduleCode}'";

                var batch = Query<BatchControlEntity>(query);

                return batch.Select(x => x.BatchTableName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateBTable(string bTableName)
        {
            try
            {
                var query = $@"CREATE TABLE {bTableName} (
	                            [SysKey] [varchar](50) PRIMARY KEY NOT NULL ,
	                            [CreatedDate] [int] NULL ,
	                            [CreatedTime] [varchar](10) NULL ,
	                            [ModifiedDate] [int] NULL ,
	                            [ModifiedTime] [varchar](10) NULL,
	                            [LoginUser] [varchar](150) NULL ,
	                            [ModifiedUser] [varchar](150) NULL ,
                                [Auto] [int] IDENTITY(1,1) NOT NULL,
	                            [RecordStatus] [int] NOT NULL ,
	                            [ParentId] [varchar](50) NULL ,
	                            [R1] [varchar](50) NOT NULL,
	                            [VoucherID] [int] ,
                                [Prefix] [varchar](15) ,
                                [Postfix] [varchar](15) ,
                                [EntryDescription] [varchar](250) ,
	                            [LineDescription] [varchar](250) ,
	                            [PostDate] [int] NOT NULL,
	                            [SecondRef] [varchar](50) ,
                                [ModuleCode] [varchar](15) NOT NULL,
	                            [YCPC] [varchar](15) NOT NULL,
	                            [BatchNo] [varchar](50) NOT NULL,
	                            [AccSK] [varchar](50) REFERENCES ACC001(SysKey) NOT NULL,
	                            [TransactionAmount] [decimal](18, 10) NOT NULL,
	                            [TaxSK] [varchar](50) ,
	                            [TaxPercent] [decimal](18, 10) ,
	                            [IsTaxInclusive] [bit] ,
	                            [TaxAmount] [decimal](18, 10) ,
	                            [SourceHeaderTransSK] [varchar](50) NULL ,
	                            [ReversalRefNo] [varchar](50) NULL,
	                            [TransType] [int] NOT NULL,
                                [TransTypeDescription] [varchar](50) NULL,
	                            [IsAutoOpening] [bit] NOT NULL,
                                [JobSK] [varchar](50) NULL,
	                            [DepartmentSK] [varchar](50) NULL,
                                [HasTaxLine] [bit] NULL,
                                [CURRSK] [varchar](50) NOT NULL,
                                [CURRATE] [decimal](18,10) NOT NULL,
                                [JobTransactionAmount] [decimal](18,10) NOT NULL,
                                [CURROP] [varchar](10) NOT NULL)";

                DbExcutor.ExcuteNonQuery(query, CommonConstant.AccountConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
