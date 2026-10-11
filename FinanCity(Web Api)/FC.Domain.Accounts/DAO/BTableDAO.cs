namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;

    public class BTableDAO : DAO<BTableEntity>
    {
        #region Methods

        public override IEnumerable<BTableEntity> Finds(Expression<Func<BTableEntity, bool>> filter)
        {
            try
            {
                return Finds(selector: null, filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override void Delete(Expression<Func<BTableEntity, bool>> filter, string table = "")
        {
            try
            {
                DbExcutor<BTableEntity>.ExcuteNonQuery($"DELETE FROM {table} WHERE {filter.ToSQL().Replace("[]", $"[{table}]")}");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<JournalSummaryDTO> GetDataPager(int currentPage, int pageSize,
         Expression<Func<JournalSummaryDTO, bool>> where = null)
        {
            try
            {
                return ExcutePagerResult<JournalSummaryDTO>($@"{JournalSummaryQuery(where)}
                    ORDER BY 
                    {nameof(JournalSummaryDTO.R1)} DESC, 
                    {nameof(JournalSummaryDTO.RecordStatus)}", currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public decimal GetOpeningBalance(int date, string accsk)
        {
            var query = "";
            var batch = new List<string>();

            try
            {
                if (accsk.StartsWith("1") || accsk.StartsWith("2") || accsk.StartsWith("3"))
                    batch = GetBTableByDateRange(20150101, date, null);
                else
                    batch = GetBTableByDateRange(date, date, null);                

                for (int i = 0; i < batch.Count; i++)
                {
                    query += $@"SELECT 
                            ISNULL(SUM(TransactionAmount),0) [TransactionAmount]
                            FROM {batch[i]}
                            WHERE RecordStatus = 8
                            AND IsAutoOpening = 0
                            AND PostDate < {date}
                            AND AccSK = '{accsk}'";

                    if (i != batch.Count - 1)
                    {
                        query += " UNION ALL ";
                    }
                }

                query = $@"SELECT ISNULL(SUM(tbl.TransactionAmount),0) FROM ({query}) [tbl]";
                return ((decimal)DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection)).ToRoundUp();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //public decimal GetOpeningBalance(int fromDate, int toDate, string accsk)
        //{
        //    try
        //    {
        //        if (accsk.StartsWith("1") || accsk.StartsWith("2") || accsk.StartsWith("3"))
        //        {

        //        }


        //        var query = "";
        //        var batch = GetBTableByDateRange(fromDate,
        //            toDate, null);

        //        for (int i = 0; i < batch.Count; i++)
        //        {
        //            query += $@"SELECT 
        //                    ISNULL(SUM(TransactionAmount),0) [TransactionAmount]
        //                    FROM {batch[i]}
        //                    WHERE RecordStatus = 8
        //                    AND IsAutoOpening = 0
        //                    AND PostDate < {toDate}
        //                    AND AccSK = '{accsk}'";

        //            if (i != batch.Count - 1)
        //            {
        //                query += " UNION ALL ";
        //            }
        //        }

        //        query = $@"SELECT ISNULL(SUM(tbl.TransactionAmount),0) FROM ({query}) [tbl]";

        //        return ((decimal)DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection)).ToRoundUp();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        public IDataPagerResult<JournalSummaryDTO> GetAuditDataPager(int currentPage, int pageSize,
            Expression<Func<JournalSummaryDTO, bool>> where = null)
        {
            try
            {
                return ExcutePagerResult<JournalSummaryDTO>($@"{JournalSummaryQuery(where)}
                    ORDER BY 
                    {nameof(JournalSummaryDTO.R1)} DESC, 
                    {nameof(JournalSummaryDTO.ParentId)},
                    {nameof(JournalSummaryDTO.RecordStatus)}",
                    currentPage, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<JournalSummaryDTO> GetSummary(Expression<Func<JournalSummaryDTO, bool>> where = null)
        {
            try
            {
                return Query<JournalSummaryDTO>($@"{JournalSummaryQuery(where)}
                    ORDER BY {nameof(JournalSummaryDTO.R1)} DESC,
                    {nameof(JournalSummaryDTO.RecordStatus)}");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetBatchTableName(string yc, string moduleCode)
        {
            try
            {
                var query = $@"SELECT MAX({With<BatchControlEntity>(x => x.BatchTableName)}) FROM {Table<BatchControlEntity>()} WHERE 
                {With<BatchControlEntity>(x => x.YC)} LIKE '%{yc}%' AND 
                {With<BatchControlEntity>(x => x.ModuleCode)} = '{moduleCode}'";

                return DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection).ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetBatchNo(string yc, string moduleCode)
        {
            try
            {
                var query = $@"SELECT MAX({With<BatchControlEntity>(x => x.SysKey)}) FROM {Table<BatchControlEntity>()} WHERE 
                {With<BatchControlEntity>(x => x.YC)} LIKE '%{yc}%' AND 
                {With<BatchControlEntity>(x => x.ModuleCode)} = '{moduleCode}'";

                return DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection).ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private string JournalSummaryQuery(Expression<Func<JournalSummaryDTO, bool>> where = null)
        {
            try
            {
                var query = "";
                int? date1 = this.MinGLDate();
                int? date2 = this.MaxGLDate();

                var a = where.CreateObject((member, value) =>
                {
                    if (member == $"{nameof(JournalSummaryDTO.PostDate)}"
                    && date1 != null)
                    {
                        if (value != "0")
                            date1 = int.Parse(value);
                    }

                    else if (member == $"{nameof(JournalSummaryDTO.PostDate)}"
                    && date2 != null)
                    {
                        if (value != "0")
                            date2 = int.Parse(value);
                    }
                });

                var batch = GetBTableByDateRange(date1.Value, 
                    date2.Value, 
                    a.ModuleCode);

                for (int i = 0; i < batch.Count; i++)
                {
                    query += $@"SELECT R1 [{nameof(JournalSummaryDTO.R1)}],
                            SecondRef [{nameof(JournalSummaryDTO.SecondRef)}],
                            PostDate [{nameof(JournalSummaryDTO.PostDate)}],
                            ParentID [{nameof(JournalSummaryDTO.ParentId)}],
                            BatchNo [{nameof(JournalSummaryDTO.BatchNo)}],
                            ModuleCode [{nameof(JournalSummaryDTO.ModuleCode)}],
                            YCPC [{nameof(JournalSummaryDTO.YC)}],
                            EntryDescription [{nameof(JournalSummaryDTO.Description)}],
                            IsAutoOpening [{nameof(JournalSummaryDTO.IsOpening)}],
            
                            CASE
                              WHEN ModifiedUser IS NOT NULL THEN ModifiedUser
                            ELSE  LoginUser
                            END [{nameof(JournalSummaryDTO.LoginUser)}],

                            CreatedDate [{nameof(JournalSummaryDTO.CreatedDate)}],
                            
                            dbo.ConvertCustomTimeFormat(CreatedTime) [{nameof(JournalSummaryDTO.CreatedTime)}],
                   
                            CURRSK [{nameof(JournalSummaryDTO.CURRSK)}],
                            CURRATE [{nameof(JournalSummaryDTO.CURRATE)}],
                            CURROP [{nameof(JournalSummaryDTO.CURROP)}],
                            CASE WHEN TransactionAmount > 0 THEN ROUND(TransactionAmount,2) END [{nameof(JournalSummaryDTO.TotalDebit)}],
                            CASE WHEN TransactionAmount < 0 THEN ROUND(TransactionAmount,2) * (-1) END [{nameof(JournalSummaryDTO.TotalCredit)}],
                            RecordStatus [{nameof(JournalSummaryDTO.RecordStatus)}]
                            FROM {batch[i]}";

                    if (i != batch.Count - 1)
                    {
                        query += " UNION ALL ";
                    }
                }
               
                query = $@"SELECT  
                     {nameof(JournalSummaryDTO.R1)},
                     {nameof(JournalSummaryDTO.SecondRef)},
                     {nameof(JournalSummaryDTO.PostDate)},
                     {nameof(JournalSummaryDTO.ParentId)},
                     {nameof(JournalSummaryDTO.Description)},
                     {nameof(JournalSummaryDTO.BatchNo)},
                     {nameof(JournalSummaryDTO.IsOpening)},
                     {nameof(JournalSummaryDTO.ModuleCode)},
                     {nameof(JournalSummaryDTO.CreatedDate)},
                     {nameof(JournalSummaryDTO.CreatedTime)},
                     {nameof(JournalSummaryDTO.LoginUser)},
                     {nameof(JournalSummaryDTO.CURRSK)},
                     {nameof(JournalSummaryDTO.CURRATE)},
                     {nameof(JournalSummaryDTO.CURROP)},
                     SUBSTRING(YC,1,2) {nameof(JournalSummaryDTO.YC)},
                     SUM(ROUND(ISNULL({nameof(JournalSummaryDTO.TotalCredit)},0),2)) {nameof(JournalSummaryDTO.TotalCredit)},
                     SUM(ROUND(ISNULL({nameof(JournalSummaryDTO.TotalDebit)},0),2)) {nameof(JournalSummaryDTO.TotalDebit)},
                     {nameof(JournalSummaryDTO.RecordStatus)}
                     FROM ({query}) [tbl]
                     WHERE ({where.ToSQL()})
                     GROUP BY
                     {nameof(JournalSummaryDTO.R1)},
                     {nameof(JournalSummaryDTO.SecondRef)},
                     {nameof(JournalSummaryDTO.PostDate)},
                     {nameof(JournalSummaryDTO.ParentId)},
                     {nameof(JournalSummaryDTO.Description)},
                     {nameof(JournalSummaryDTO.BatchNo)},
                     {nameof(JournalSummaryDTO.ModuleCode)},
                     {nameof(JournalSummaryDTO.IsOpening)},
                     {nameof(JournalSummaryDTO.YC)},
                     {nameof(JournalSummaryDTO.RecordStatus)},
                     {nameof(JournalSummaryDTO.CreatedDate)},
                     {nameof(JournalSummaryDTO.CreatedTime)},
                     {nameof(JournalSummaryDTO.LoginUser)},
                     {nameof(JournalSummaryDTO.CURRSK)},
                     {nameof(JournalSummaryDTO.CURROP)},
                     {nameof(JournalSummaryDTO.CURRATE)}";

                return query;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private List<string> GetBTableByDateRange(int fromDate,
            int toDate,
            string moduleCode)
        {
            try
            {
                var result = new List<string>();
                var startYC = DbExcutor.ExcuteScalar($"SELECT YC1 FROM GetYCRange('{fromDate}', '{toDate}')", CommonConstant.AccountConnection);
                var endYC = DbExcutor.ExcuteScalar($"SELECT YC2 FROM GetYCRange('{fromDate}','{toDate}')", CommonConstant.AccountConnection);

                var query = $@"SELECT * FROM {Table<BatchControlEntity>()} 
                WHERE {With<BatchControlEntity>(x => x.YC)} BETWEEN '{startYC}' AND '{endYC}'";

                if (!string.IsNullOrWhiteSpace(moduleCode))
                {
                    query += $@"AND {With<BatchControlEntity>(x => x.ModuleCode)} = '{moduleCode}'";
                }

                var batch = DbExcutor<BatchControlEntity>.ExcuteReader(query);
                return batch.Select(x => x.BatchTableName).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int MinGLDate()
        {
            try
            {
                var query = $@"SELECT MIN({With<AccountingPeriodEntity>(x => x.Date1)}) FROM {Table<AccountingPeriodEntity>()}";
                return (int)DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private int MaxGLDate()
        {
            try
            {
                var query = $@"SELECT MAX({With<AccountingPeriodEntity>(x => x.Date2)}) FROM {Table<AccountingPeriodEntity>()}";
                return (int)DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
