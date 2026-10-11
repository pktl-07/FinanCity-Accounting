namespace FC.Domain.Version.DAO
{
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.1.0.12", Description = @"")]
	public class V_1_1_0_12_DAO : DAO<VersionEntity>, IEssVersion
	{
		#region Ctor

		public V_1_1_0_12_DAO()
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
							WHERE YC BETWEEN @StartYC AND @EndYC

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
								                ELSE 0.00 END [Debit],
								                CASE 
								                WHEN ' + @bTable +'.TransactionAmount < 0 THEN ROUND(' + @bTable +'.TransactionAmount,2) 
								                ELSE 0.00 END [Credit]
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
	                            0 [OpeningBalance],
	                            ISNULL(SUM(ROUND(t.Debit,2)),0) [Debit],
	                            ISNULL(SUM(ROUND(t.Credit,2)),0) [Credit],
	                            0 [ClosingBalance]
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
                             , 0.00 [OpeningBalance]
                             , ISNULL([tbl].Debit,0.00) [Debit]
                             , ISNULL([tbl].Credit,0.00) [Credit]
							 , 0.00 [ClosingBalance]
                             , ISNULL([tbl].ClosingBalance,0) [ClosingBalance]
                              FROM (' + @Query + ') [tbl] RIGHT JOIN ACC001 ON ACC001.SysKey = [tbl].ACCSK WHERE ACC001.AccountType=1'

                              IF(@AccSK IS NOT NULL)
		                            SELECT @Query += ' AND ACC001.SysKey='+@AccSK;

						EXEC(@Query);
			

			", CommonConstant.AccountConnection);

			DbExcutor.ExcuteNonQuery(@"ALTER PROC [dbo].[FR_007](@FromDate varchar(15),@ToDate varchar(15),@AccSK varchar(50))
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

	                    EXEC(@Query);",CommonConstant.AccountConnection);

			DbExcutor.ExcuteNonQuery(@"
			ALTER PROC [dbo].[FR_008](@FromDate varchar(15),@ToDate varchar(15),@AccSK varchar(50),@GLRef varchar(50),@SecondRef varchar(50))
                            AS
                              IF OBJECT_ID('tempdb..#BTempTable') IS NOT NULL
	                            DROP TABLE #BTempTable

                            CREATE TABLE #BTempTable
                            (
	                            PostDate varchar(50),
	                            AccSK varchar(50),
								AccCode varchar(50),
								AccDescription varchar(50),
	                            GLRef varchar(50),
								SecondRef varchar(50),
								Src varchar(50),
								Debit decimal(18,10),
	                            Credit decimal(18,10),
								TransType varchar(50),	
								RecordStatus int
                            )
                            --SELECT BTable's Name ACC005 And Add to temp #BTableName table.
                            IF OBJECT_ID('tempdb..#BTableName') IS NOT NULL
	                            DROP TABLE #BTableName

							DECLARE @StartYC varchar(50) = (SELECT YC1 FROM GetYCRange(@FromDate,@ToDate));
							DECLARE @EndYC varchar(50) = (SELECT YC2 FROM GetYCRange(@FromDate,@ToDate));
							
                            SELECT BatchTableName,ROW_NUMBER() OVER(ORDER BY BatchTableName) as rownumber INTO #BTableName 
							FROM ACC005 WHERE YC BETWEEN @StartYC AND @EndYC;
							

                            DECLARE @rowcount int;
                            DECLARE @bTable varchar(50);
                            SELECT @rowcount =1;
                            DECLARE @Query varchar(MAX);
                            WHILE @rowcount <= (Select COUNT(BatchTableName) FROM #BTableName)
                            BEGIN
	                            SELECT @bTable = (SELECT BatchTableName FROM #BTableName WHERE rownumber=@rowcount);
	                            SELECT @Query = 'INSERT INTO #BTempTable(PostDate,AccSK,AccCode,AccDescription,GLRef,SecondRef,Src,Debit,Credit,TransType,RecordStatus)
								                               SELECT 
								                               '+ @bTable +'.PostDate,
								                               '+ @bTable +'.AccSK,
															    ACC001.AccCode [AccCode],
																ACC001.AccDescription [AccDescription],
															   '+ @bTable+'.R1 [GLRef],
															   '+ @bTable+'.SecondRef [SecondRef],
															   '+ @bTable+'.ModuleCode [Src],
								                               CASE 
								                               WHEN ' + @bTable +'.TransactionAmount > 0 THEN ROUND(' + @bTable +'.TransactionAmount,2) 
								                               ELSE 0 END [Debit],
								                               CASE 
								                               WHEN ' + @bTable +'.TransactionAmount < 0 THEN ROUND(' + @bTable +'.TransactionAmount,2) 
								                               ELSE 0 END [Credit],
															   '+ @bTable +'.TransTypeDescription [TransType],
															   '+ @bTable +'.RecordStatus [RecordStatus]
								                               FROM ' + @bTable + '
								                               INNER JOIN ACC001 ON ACC001.SysKey = ' + @bTable + '.AccSK
								                               WHERE ' + @bTable + '.RecordStatus >= 6';
															  

								                            IF(@AccSK IS NOT NULL)
									                            SELECT @Query += ' AND ' + @bTable +'.AccSK='+@AccSK;

															IF(@GLRef IS NOT NULL)
									                            SELECT @Query += ' AND ' + @bTable +'.R1='+@GLRef;

    														IF(@SecondRef IS NOT NULL)
									                            SELECT @Query += ' AND ' + @bTable +'.SecondRef='+@SecondRef;

							                               --  IF(@rowcount > 1)
									                             --SELECT @Query += ' AND ' + @bTable + '.IsAutoOpening <> 1';
						                            EXEC(@Query);		  
	                            SELECT @rowcount= @rowcount+1;
                            END

							SELECT dbo.ConvertCustomDateFormat(PostDate) [PostDate],
							AccSK,
							AccCode,
							AccDescription,
							GLRef,
							SecondRef,
							Src,
							Debit,
							Credit,
							TransType,
							RecordStatus
						    FROM #BTempTable WHERE #BTempTable.PostDate BETWEEN @FromDate AND @ToDate ORDER BY GLRef
			", CommonConstant.AccountConnection);

			DbExcutor.ExcuteNonQuery(@"
			ALTER PROC [dbo].[FR_010](@FromDate VARCHAR(15),
				@ToDate VARCHAR(15),
				@AccSK VARCHAR(50),
				@SecondRef VARCHAR(50),
				@Ref VARCHAR(50))
				AS

			IF OBJECT_ID('tempdb..#BTempTable') IS NOT NULL
			BEGIN
				DROP TABLE #BTempTable
			END

		-- tempory batch table to store BTable results.
		CREATE TABLE #BTempTable
		(
			GLNo VARCHAR(50),
			PostDate VARCHAR(50),
			ModuleCode VARCHAR(50),
			AccCode VARCHAR(50),
			TransactionAmount DECIMAL(18,10),
			JOBSK VARCHAR(50),
			JobCode VARCHAR(50),
			EntryDescription VARCHAR(MAX),
			RecordStatus INT,
			SecondRef VARCHAR(50),
			CreatedDate INT,
			CreatedTime INT,
			ModifiedDate INT,
			ModifiedTime INT,
			LoginUser VARCHAR(50),
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
		SELECT @Query = 'INSERT INTO #BTempTable(GLNo,PostDate,ModuleCode,AccCode,TransactionAmount,JOBSK,JobCode,EntryDescription,RecordStatus,SecondRef,CreatedDate,CreatedTime,ModifiedDate,ModifiedTime,LoginUser)
							SELECT 
							'+ @bTable +'.R1,
						    dbo.ConvertCustomDateFormat('+ @bTable +'.PostDate),
							'+ @bTable +'.ModuleCode,
							'+ @bTable +'.AccCode [AccCode],
							ACC001.
							ROUND('+ @bTable +'.TransactionAmount,2) [TransactionAmount],
								JOB.SysKey [JOBSK],
								JOB.R1 [JobCode],
							'+ @bTable +'.LineDescription,
							'+ @bTable +'.RecordStatus,
							'+ @bTable +'.SecondRef,
							dbo.ConvertCustomDateFormat('+ @bTable +'.CreatedDate),
							dbo.ConvertCustomTimeFormat('+ @bTable +'.CreatedTime),
							dbo.ConvertCustomDateFormat('+ @bTable +'.ModifiedDate),
							dbo.ConvertCustomTimeFormat('+ @bTable +'.ModifiedTime),
							'+ @bTable +'.LoginUser
								FROM ' + @bTable + ' 
								LEFT JOIN JOB ON JOB.SysKey =' + @bTable + '.JobSK
								INNER JOIN ACC001 ON ACC001.SysKey=' + @bTable + '.AccSK
								WHERE ' + @bTable + '.RecordStatus >= 6
							    AND ' + @bTable + '.TransactionAmount <> 0 ';
											
													
						IF(@SecondRef IS NOT NULL)
							SELECT @Query += ' AND ' + @bTable + '.SecondRef LIKE ' + '' + '%' +@SecondRef + '%' +'';

						IF(@Ref IS NOT NULL)
							SELECT @Query += ' AND ' + @bTable + '.R1 LIKE ' + '' + '%' +@Ref + '%' + '';

						IF(@AccSK IS NOT NULL)
							SELECT @Query += ' AND ' + @bTable + '.AccSK=' + @AccSK;

						                            
						EXEC(@Query);		  
			SELECT @rowcount= @rowcount+1;
		END

		SELECT * FROM #BTempTable ORDER BY GLNo
			", CommonConstant.AccountConnection);

			DbExcutor.ExcuteNonQuery(@"
		ALTER PROC [dbo].[R0001](@FromDate varchar(15),@ToDate varchar(15), @TaxSK varchar(50))
                AS

                IF OBJECT_ID('tempdb..#BTempTable') IS NOT NULL
                BEGIN
                DROP TABLE #BTempTable
                DROP TABLE #BTempTransformTable
                END

	                --Tempory batch table to store BTable results.
	                CREATE TABLE #BTempTable
	                (
			                [Date] varchar(50),
			                [TaxSK] varchar(50),
			                [TaxCode] varchar(50),
			                [TransactionAmount] decimal(18,10),
			                [GSTIN] decimal(18,10),
			                [GSTOUT] decimal(18,10), 
			                [TaxPercent] decimal(18,2),
			                [Ref] varchar(50),
			                [SecondRef] varchar(50),
			                [Description] varchar(MAX),
			                [InvoiceAmt] decimal(18,10),
	                )
							
                --SELECT BTable's Name ACC005 And Add to temp #BTableName table.
                IF OBJECT_ID('tempdb..#BTableName') IS NOT NULL
	                DROP TABLE #BTableName

                DECLARE @StartYC varchar(50) = (SELECT YC1 FROM GetYCRange(@FromDate,@ToDate));
                DECLARE @EndYC varchar(50) = (SELECT YC2 FROM GetYCRange(@FromDate,@ToDate));

                SELECT BatchTableName,ROW_NUMBER() OVER(ORDER BY BatchTableName) as rownumber INTO #BTableName FROM ACC005 WHERE YC BETWEEN @StartYC AND @EndYC

                DECLARE @rowcount int; -- counter
                DECLARE @bTable varchar(50); -- batch table name
                SELECT  @rowcount = 1; -- set counter
                DECLARE @Query varchar(MAX); -- variable to hould dynamic query string

                -- AS Batch Table are seperating and re orgainizing into one hash temp table
                WHILE @rowcount <= (Select COUNT(BatchTableName) FROM #BTableName)
                BEGIN

                SELECT @bTable = (SELECT BatchTableName FROM #BTableName WHERE rownumber=@rowcount);
                SELECT @Query = 'INSERT INTO #BTempTable(Date,TaxSK,TaxCode,TransactionAmount,GSTIN,GSTOUT,TaxPercent,Ref,SecondRef,Description,InvoiceAmt)
					                SELECT 
					                '+ @bTable +'.PostDate [Date],
					                ACC008.SysKey [TaxSK],
					                ACC008.TaxCode [TaxCode],
					                ROUND('+ @bTable +'.TransactionAmount,2) [TransactionAmount],
								                
					                CASE 
					                WHEN '+ @bTable +'.TaxPercent = 0 THEN 0
									WHEN (ACC008.TaxType = 3 OR ACC008.TaxType = 1) THEN ROUND('+ @bTable +'.TransactionAmount,2) 
									ELSE 0 
					                END [GSTIN],

					                CASE
					                WHEN '+ @bTable +'.TaxPercent = 0 THEN 0
									WHEN ACC008.TaxType = 2 THEN ROUND('+ @bTable +'.TransactionAmount,2) * (-1)
									ELSE 0 
					                END [GSTOUT],

					                '+ @bTable +'.TaxPercent [TaxPercent],
					                '+ @bTable +'.R1 [Ref],
					                '+ @bTable +'.SecondRef [SecondRef],
					                '+ @bTable +'.LineDescription [Description],
							    	'+ @bTable +'.TaxAmount [InvoiceAmt]
					        
				                    FROM ' + @bTable + '
					                INNER JOIN ACC008 ON ACC008.SysKey = '+ @bTable +'.TaxSK
									--AND '+ @bTable +'.ReversalStatus = 0
					                WHERE ' + @bTable + '.RecordStatus = 8 AND ' + @bTable + '.PostDate BETWEEN ' + @FromDate + ' AND ' + @ToDate;
								
					                IF @TaxSK IS NOT NULL
						                SELECT @Query += ' AND ' + @bTable + '.TaxSK ='+ '''' + @TaxSK + '''';

				                EXEC(@Query);		  
	 
	                SELECT @rowcount= @rowcount + 1;
                END

							
                SELECT @Query +='SELECT dbo.ConvertCustomDateFormat(#BTempTable.[Date]) [Date],
                ACC008.TaxCode,
                ISNULL(ROUND(TransactionAmount,2),0) [TransactionAmount],
                ISNULL(ROUND(GSTIN,2),0) [GSTIN],
                ISNULL(ROUND(GSTOUT,2),0) [GSTOUT],
                ISNULL(ACC008.TaxPercent,0) [TaxPercent],
                Ref,
                SecondRef,
                #BTempTable.Description,
				ISNULL(InvoiceAmt,0) [InvoiceAmt]
			    FROM #BTempTable 
                RIGHT JOIN ACC008 ON ACC008.SysKey = #BTempTable.TaxSK
                WHERE 1=1 ';

	                IF @TaxSK IS NOT NULL
		                SELECT @Query += ' AND ACC008.SysKey ='+ '''' + @TaxSK + '''';

                SELECT @Query += ' ORDER BY #BTempTable.Ref';

                EXEC(@Query);
", CommonConstant.AccountConnection);
		
		}

		#endregion
	}
}

