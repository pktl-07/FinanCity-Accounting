namespace FC.Domain.Version.DAO
{
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.1.0.16", Description = @"")]
	public class V_1_1_0_16_DAO : DAO<VersionEntity>, IEssVersion
	{
		#region Ctor

		public V_1_1_0_16_DAO()
		{
		}

		#endregion

		#region Methods

		public void Excute()
		{
			DbExcutor.ExcuteNonQuery(@"
							ALTER PROC [dbo].[FR_005](@FromDate varchar(15),@ToDate varchar(15),@AccSK varchar(50))
                            AS

                              IF OBJECT_ID('tempdb..#BTempTable') IS NOT NULL
                              BEGIN
	                            DROP TABLE #BTempTable
	                            DROP TABLE #BTempTransformTable
                              END
                            -- tempory batch table to store BTable results.
                            CREATE TABLE #BTempTable
                            (
	                            RowNo INT,
	                            GLNo VARCHAR(50),
	                            PostDate VARCHAR(50),
	                            ModuleCode VARCHAR(50),
	                            AccSK VARCHAR(50),
	                            TransactionAmount DECIMAL(18,10),
	                            JOBSK VARCHAR(50),
	                            JobCode VARCHAR(50),
								EntryDescription VARCHAR(MAX),
								RecordStatus INT,
								SecondRef VARCHAR(50),
								CreatedDate INT,
								CreatedTime INT,
								OpeningBalance DECIMAL(18,10),
								ClosingBalance DECIMAL(18,10)

                            )
							
                            --SELECT BTable's Name ACC005 And Add to temp #BTableName table.
                            IF OBJECT_ID('tempdb..#BTableName') IS NOT NULL
	                            DROP TABLE #BTableName

    						DECLARE @StartYC VARCHAR(50) = (SELECT YC1 FROM GetYCRange(@FromDate,@ToDate));
							DECLARE @EndYC VARCHAR(50) = (SELECT YC2 FROM GetYCRange(@FromDate,@ToDate));

                            SELECT BatchTableName,ROW_NUMBER() OVER(ORDER BY BatchTableName) as rownumber INTO #BTableName 
							FROM ACC005 WHERE YC BETWEEN @StartYC AND @EndYC;

                            DECLARE @rowcount INT; -- counter
                            DECLARE @bTable VARCHAR(50); -- batch table name
                            SELECT @rowcount = 1; -- set counter
                            DECLARE @Query VARCHAR(MAX); -- variable to hould dynamic query string

                            -- AS Batch Table are seperating and re orgainizing into one hash temp table
                            WHILE @rowcount <= (SELECT COUNT(BatchTableName) FROM #BTableName)
                            BEGIN

                            SELECT @bTable = (SELECT BatchTableName FROM #BTableName WHERE rownumber=@rowcount);
                            SELECT @Query = 'INSERT INTO #BTempTable(GLNo,PostDate,ModuleCode,AccSK,TransactionAmount,JOBSK,JobCode,EntryDescription,RecordStatus,SecondRef,CreatedDate,CreatedTime,OpeningBalance,ClosingBalance)
					                            SELECT 
					                            '+ @bTable +'.R1,
					                            '+ @bTable +'.PostDate,
					                            '+ @bTable +'.ModuleCode,
					                            '+ @bTable +'.AccSK,
					                            ROUND('+ @bTable +'.TransactionAmount,2) [TransactionAmount],
					                             JOB.SysKey,
					                             JOB.R1,
												'+ @bTable +'.LineDescription,
												'+ @bTable +'.RecordStatus,
												'+ @bTable +'.SecondRef,
												'+ @bTable +'.CreatedDate,
												'+ @bTable +'.CreatedTime,
												 0.00,
												 0.00 
					                             FROM ' + @bTable + ' LEFT JOIN JOB ON JOB.SysKey =' + @bTable + '.JobSK
					                             WHERE ' + @bTable + '.RecordStatus = 8 AND ' + @bTable + '.TransactionAmount <> 0 ';
							  
				                            IF(@AccSK IS NOT NULL)
					                            SELECT @Query += ' AND ' + @bTable + '.AccSK=' + @AccSK;

					                            --IF(@rowcount > 1)
						                      SELECT @Query += ' AND ' + @bTable + '.IsAutoOpening <> 1';

						                            EXEC(@Query);		  
	                            SELECT @rowcount= @rowcount+1;
                            END

                            -- Re arranging to assing correct row no and put it into #BTempTransformTable, the final query result should retrive from #BTempTransformTable
                            SELECT *
                            INTO #BTempTransformTable
                            FROM (SELECT RowNo=ROW_NUMBER() OVER(ORDER BY CreatedDate,CreatedTime),GLNo, PostDate, ModuleCode,AccSK,TransactionAmount,JOBSK,JobCode,EntryDescription,RecordStatus,SecondRef,CreatedDate,CreatedTime,OpeningBalance,ClosingBalance FROM #BTempTable) [t]

                            -- preparing for the result set that come out producing opening and closing balance base on the accsk and rowno, each row number is unique and no need to worry about duplication.
                            SELECT @Query =' SELECT 
	                            t.GLNo,
	                            t.ModuleCode,
	                            ACC001.SysKey [AccSK],
	                            ACC001.AccCode [AccCode],
	                            ACC001.AccDescription [AccDescription],
	                            dbo.ConvertCustomDateFormat(PostDate) [PostDate],
	                            t.JOBSK [JOBSK],
	                            t.JobCode [JobCode],
	                            0.00 [OpeningBalance],
	                            CASE 
	                            WHEN t.TransactionAmount > 0 THEN ROUND(t.TransactionAmount,2)
	                            ELSE 
	                            0.00 END [Debit],
	                            CASE 
	                            WHEN t.TransactionAmount < 0 THEN ROUND(t.TransactionAmount,2)
	                            ELSE 
	                            0.00 END [Credit],
	                            0.00 [ClosingBalance],
								t.EntryDescription [EntryDescription],
								t.RecordStatus [RecordStatus],
								t.SecondRef,
								t.CreatedDate,
								t.CreatedTime
 	                            FROM ACC001 
								INNER JOIN #BTempTransformTable [t] ON ACC001.SysKey = t.AccSK 
								WHERE 1=1 
								AND t.PostDate BETWEEN ' + @FromDate + ' AND ' + @ToDate;

                            -- right join with ACC001 as our client need to view eventhough some accounts which never used. 
	                            SELECT @Query = ' SELECT [tbl].GLNo [GLNo]
	                             , [tbl].ModuleCode [ModuleCode]
	                             , ACC001.SysKey [AccSK]
	                             , ACC001.AccCode [AccCode]
	                             , ACC001.AccDescription [AccDescription]
	                             , [tbl].PostDate [PostDate]
	                             , [tbl].JOBSK [JOBSK]
	                             , [tbl].JobCode [JobCode]
								
								 ,0.00 [OpeningBalance]
	                            
								 , ISNULL([tbl].Debit,0) [Debit]
	                             , ISNULL([tbl].Credit,0) [Credit]
								 ,0.00 [ClosingBalance]

	                             --, ISNULL([tbl].ClosingBalance,0) [ClosingBalance] 
								 , [tbl].EntryDescription [EntryDescription]
								 , [tbl].RecordStatus [RecordStatus]
								 , [tbl].SecondRef [SecondRef]
								 , [tbl].CreatedDate [CreatedDate]
								 , [tbl].CreatedTime [CreatedTime]

								 FROM (' + @Query + ') [tbl] 
								 RIGHT JOIN ACC001 ON ACC001.SysKey = [tbl].AccSK 
								 WHERE ACC001.AccountType=1'
	 	 
	                            IF(@AccSK IS NOT NULL)
		                            SELECT @Query += ' AND ACC001.SysKey='+@AccSK;

								SELECT @Query += ' ORDER BY [tbl].CreatedDate, [tbl].CreatedTime';

	                            EXEC(@Query);
			", CommonConstant.AccountConnection);

			DbExcutor.ExcuteNonQuery(@"
				ALTER PROC [dbo].[FR_006](@FromDate varchar(15),@ToDate varchar(15),@AccSK varchar(50))
                            AS
                              IF OBJECT_ID('tempdb..#BTempTable') IS NOT NULL
	                            DROP TABLE #BTempTable

                            CREATE TABLE #BTempTable
                            (
	                            PostDate varchar(50),
	                            AccSK varchar(50),
	                            Debit decimal(18,10),
	                            Credit decimal(18,10),
								
                            )
                            --SELECT BTable's Name ACC005 And Add to temp #BTableName table.
                            IF OBJECT_ID('tempdb..#BTableName') IS NOT NULL
	                            DROP TABLE #BTableName

							DECLARE @StartYC varchar(50) = (SELECT YC1 FROM GetYCRange(@FromDate,@ToDate));
							DECLARE @EndYC varchar(50) = (SELECT YC2 FROM GetYCRange(@FromDate,@ToDate));
                            SELECT BatchTableName,ROW_NUMBER() OVER(ORDER BY BatchTableName) as rownumber INTO #BTableName 
							FROM ACC005
							WHERE YC IN (@StartYC,@EndYC)

                            DECLARE @rowcount int;
                            DECLARE @bTable varchar(50);
                            SELECT @rowcount =1;
                            DECLARE @Query varchar(MAX);
                            WHILE @rowcount <= (Select COUNT(BatchTableName) FROM #BTableName)
                            BEGIN

	                            SELECT @bTable = (SELECT BatchTableName FROM #BTableName WHERE rownumber=@rowcount);
	                            SELECT @Query = 'INSERT INTO #BTempTable(PostDate,AccSK,Debit,Credit)
								                SELECT 
								                '+ @bTable +'.PostDate,
								                '+ @bTable +'.AccSK,
								                CASE 
								                WHEN ' + @bTable +'.TransactionAmount > 0 THEN ROUND(' + @bTable +'.TransactionAmount,2) 
								                ELSE 0 END [Debit],
								                CASE 
								                WHEN ' + @bTable +'.TransactionAmount < 0 THEN ROUND(' + @bTable +'.TransactionAmount,2) 
								                ELSE 0 END [Credit]
								                FROM ' + @bTable + '
								                RIGHT JOIN ACC001 ON ACC001.SysKey = ' + @bTable + '.AccSK
								                WHERE ' + @bTable + '.RecordStatus = 8'
							  
								            IF(@AccSK IS NOT NULL)
									            SELECT @Query += ' AND ' + @bTable +'.AccSK='+@AccSK;
												
										   --IF(@rowcount > 1)
									       SELECT @Query += ' AND ' + @bTable + '.IsAutoOpening <> 1';

						                            EXEC(@Query);		  
	                            SELECT @rowcount= @rowcount+1;
                            END

                            SELECT @Query =' SELECT 
	                            ACC001.SysKey [AccSK],
	                            ACC001.AccCode [AccCode],
	                            ACC001.AccDescription [AccDescription],
	                            (SELECT ISNULL(SUM(ROUND(Debit,2)),0) + ISNULL(SUM(ROUND(Credit,2)),0) FROM #BTempTable WHERE AccSK = ACC001.SysKey AND PostDate <' + @FromDate +') [OpeningBalance],
	                            ISNULL(SUM(ROUND(t.Debit,2)),0) [Debit],
	                            ISNULL(SUM(ROUND(t.Credit,2)),0) [Credit],
	                            (SELECT ISNULL(SUM(ROUND(Debit,2)),0) + ISNULL(SUM(ROUND(Credit,2)),0) FROM #BTempTable WHERE AccSK = ACC001.SysKey AND PostDate <' + @ToDate +')  [ClosingBalance]
	                            FROM ACC001 
								INNER JOIN #BTempTable [t] ON ACC001.SysKey = t.AccSK  
								WHERE t.PostDate BETWEEN ' + @FromDate + ' AND ' + @ToDate + '
	                            GROUP BY 
 	                            ACC001.SysKey,
	                            ACC001.AccCode,
	                            ACC001.AccDescription';

                            SELECT @Query = ' SELECT ACC001.SysKey [AccSK]
                             , ACC001.AccCode [AccCode]
                             , ACC001.AccDescription [AccDescription]
                             , CASE 
								   WHEN [tbl].OpeningBalance IS NULL THEN (SELECT ISNULL(SUM(ROUND(Debit,2)),0) + ISNULL(SUM(ROUND(Credit,2)),0) FROM #BTempTable WHERE AccSK = ACC001.SysKey AND PostDate <' + @FromDate +') 
								   ELSE [tbl].OpeningBalance
							   END [OpeningBalance]
                             , ISNULL([tbl].Debit,0) [Debit]
                             , ISNULL([tbl].Credit,0) [Credit]
							 , CASE 
								   WHEN [tbl].ClosingBalance IS NULL THEN (SELECT ISNULL(SUM(ROUND(Debit,2)),0) + ISNULL(SUM(ROUND(Credit,2)),0) FROM #BTempTable WHERE AccSK = ACC001.SysKey AND PostDate <' + @ToDate +')
								   ELSE [tbl].ClosingBalance
								END [ClosingBalance]
                             , ISNULL([tbl].ClosingBalance,0) [ClosingBalance]
                              FROM (' + @Query + ') [tbl] RIGHT JOIN ACC001 ON ACC001.SysKey = [tbl].ACCSK WHERE ACC001.AccountType=1'

                              IF(@AccSK IS NOT NULL)
		                            SELECT @Query += ' AND ACC001.SysKey='+@AccSK;

                            EXEC(@Query);
			", CommonConstant.AccountConnection);

			DbExcutor.ExcuteNonQuery(@"
				ALTER PROC [dbo].[FR_007](@FromDate varchar(15),@ToDate varchar(15),@AccSK varchar(50))
                    AS

                    --DECLARE @FromDate varchar(15);
                    --DECLARE @ToDate varchar(15);
                    --DECLARE @AccSK varchar(15);

                    IF OBJECT_ID('tempdb..#BTempTable') IS NOT NULL
                    BEGIN
                    DROP TABLE #BTempTable
                    DROP TABLE #BTempTransformTable
                    END
                    -- tempory batch table to store BTable results.
                    CREATE TABLE #BTempTable
                    (
	                    RowNo int,
	                    GLNo varchar(50),
	                    PostDate varchar(50),
	                    ModuleCode varchar(50),
	                    AccSK varchar(50),
	                    TransactionAmount decimal(18,10),
	                    JOBSK varchar(50),
	                    JobCode varchar(50),
	                    ParentId varchar(50),
	                    Memo varchar(MAX),
						RecordStatus int,
						ReversalStatus int,
                    )

                    --SELECT BTable's Name ACC005 And Add to temp #BTableName table.
                    IF OBJECT_ID('tempdb..#BTableName') IS NOT NULL
	                    DROP TABLE #BTableName

					DECLARE @StartYC varchar(50) = (SELECT YC1 FROM GetYCRange(@FromDate,@ToDate));
					DECLARE @EndYC varchar(50) = (SELECT YC2 FROM GetYCRange(@FromDate,@ToDate));

                    SELECT BatchTableName,ROW_NUMBER() OVER(ORDER BY BatchTableName) as rownumber INTO #BTableName 
					FROM ACC005 
					WHERE YC BETWEEN @StartYC AND @EndYC
					AND ModuleCode = 'GLC'

                    DECLARE @rowcount int; -- counter
                    DECLARE @bTable varchar(50); -- batch table name
                    SELECT @rowcount = 1; -- set counter
                    DECLARE @Query varchar(MAX); -- variable to hould dynamic query string

                    -- AS Batch Table are seperating and re orgainizing into one hash temp table
                    WHILE @rowcount <= (Select COUNT(BatchTableName) FROM #BTableName)
                    BEGIN

                    SELECT @bTable = (SELECT BatchTableName FROM #BTableName WHERE rownumber=@rowcount);
                    SELECT @Query = 'INSERT INTO #BTempTable(GLNo,PostDate,ModuleCode,AccSK,TransactionAmount,JOBSK,JobCode,ParentId,Memo,RecordStatus,ReversalStatus)
				                     SELECT 
					                    '+ @bTable +'.R1,
					                    '+ @bTable +'.PostDate,
					                    '+ @bTable +'.ModuleCode,
					                    '+ @bTable +'.AccSK,
					                    ROUND('+ @bTable +'.TransactionAmount,2) [TransactionAmount],
					                    JOB.SysKey,
					                    JOB.R1,
					                    '+ @bTable +'.ParentID,
					                    '+ @bTable +'.LineDescription,
										'+ @bTable +'.RecordStatus,
										'+ @bTable +'.ReversalStatus
					                    FROM ' + @bTable + ' LEFT JOIN JOB ON JOB.SysKey =' + @bTable + '.JobSK
					                    WHERE ' + @bTable + '.RecordStatus = 8'
							  
				                    IF(@AccSK IS NOT NULL)
					                    SELECT @Query += ' AND ' + @bTable + '.AccSK=' + @AccSK;

					                    --IF(@rowcount > 1)
						                   -- SELECT @Query += ' AND ' + @bTable + '.IsAutoOpening <> 1';

						                    EXEC(@Query);		  
	                    SELECT @rowcount= @rowcount+1;
                    END

                    -- Re arranging to assing correct row no and put it into #BTempTransformTable, the final query result should retrive from #BTempTransformTable
                    SELECT *
                    INTO #BTempTransformTable
                    FROM (SELECT RowNo=ROW_NUMBER() OVER(ORDER BY AccSK,PostDate),GLNo, PostDate, ModuleCode,AccSK,TransactionAmount,JOBSK,JobCode,ParentId,Memo,RecordStatus,ReversalStatus FROM #BTempTable) [t]

                    -- preparing the query to get parent id which has contained Liablity and use as a sub query below.
                    SELECT @Query =' SELECT 
	                    [t].ParentId
 	                    FROM #BTempTransformTable [t]
	                    WHERE 1=1 
	                    AND t.AccSK LIKE ''2%'' 
                        AND t.PostDate BETWEEN ' + @FromDate + ' AND ' + @ToDate + ' GROUP BY [t].ParentId'
							    
	                    SELECT @Query =' SELECT 
	                    t.GLNo,
	                    t.ModuleCode,
	                    ACC001.SysKey [AccSK],
	                    ACC001.AccCode [AccCode],
	                    ACC001.AccDescription [AccDescription],
	                    dbo.ConvertCustomDateFormat(PostDate) [PostDate],
	                    t.JOBSK [JOBSK],
	                    t.JobCode [JobCode],
	                    (SELECT ISNULL(SUM(ROUND(TransactionAmount,2)),0) FROM #BTempTransformTable WHERE AccSK = ACC001.SysKey AND RowNo < t.RowNo) [OpeningBalance],
	                    CASE 
	                    WHEN t.TransactionAmount > 0 THEN ROUND(t.TransactionAmount,2)
	                    ELSE 
	                    0.00 END [Debit],
	                    CASE 
	                    WHEN t.TransactionAmount < 0 THEN ROUND(t.TransactionAmount,2)
	                    ELSE 
	                    0.00 END [Credit],
	                    (SELECT ISNULL(SUM(ROUND(TransactionAmount,2)),0) FROM #BTempTransformTable WHERE AccSK = ACC001.SysKey AND RowNo < t.RowNo) + ROUND(t.TransactionAmount,2) [ClosingBalance],
	                    t.Memo [Memo],
						t.RecordStatus,
						t.ReversalStatus
 	                    FROM ACC001 
	                    INNER JOIN #BTempTransformTable [t] ON ACC001.SysKey = t.AccSK 
	                    WHERE 1=1 AND ACC001.AccountType = 1 
	                    AND [t].ParentId IN (' + @Query + ')
	                    AND [t].PostDate BETWEEN ' + @FromDate + ' AND ' + @ToDate;

	                    IF(@AccSK IS NOT NULL)
		                    SELECT @Query += ' AND ACC001.SysKey='+@AccSK;

	                    EXEC(@Query);
			", CommonConstant.AccountConnection);
		}

		#endregion
	}
}

