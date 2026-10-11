namespace FC.Domain.Version.DAO
{
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.0.0.2", 
		Description = "Adding ConvertCustomTimeFormat and FR_010 Procedure.")]
	public class V_1_0_0_2_DAO : DAO<VersionEntity>, IEssVersion
	{
		public V_1_0_0_2_DAO()
		{
		}

		public void Excute()
		{
			DbExcutor.ExcuteNonQuery(@"
               
				CREATE FUNCTION [dbo].[ConvertCustomTimeFormat](@Time INT)
                RETURNS VARCHAR(15)
                BEGIN
	
	                IF @Time IS NULL
	                BEGIN
		                RETURN NULL;
	                END 

	                DECLARE @Str_time VARCHAR(50) = CAST(@Time AS VARCHAR(6));
	                IF (LEN(@Time) = 5)
	                 BEGIN
		                SELECT @Str_time = '0' + @Str_time
	                END


	                DECLARE @hour VARCHAR(2) = (SELECT SUBSTRING(@Str_time,1,2));
   	                DECLARE	@min VARCHAR(2) = (SELECT SUBSTRING(@Str_time,3,2));
	                DECLARE @second VARCHAR(2) = (SELECT SUBSTRING(@Str_time,5,2));
	                DECLARE @l_time VARCHAR(15) = @hour + ':' + @min + ':' + @second;

	                RETURN FORMAT(CAST(@l_time AS DATETIME), 'hh:mm:ss tt')
                END", "FC.Acc");

			DbExcutor.ExcuteNonQuery(@"CREATE PROC [dbo].[FR_010](@FromDate VARCHAR(15),
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
		FROM ACC005 WHERE YC IN (@StartYC,@EndYC);

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

		SELECT * FROM #BTempTable ORDER BY GLNo", "FC.Acc");
		}
	}
}
