namespace FC.Domain.Version.DAO
{
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.1.0.14", Description = @"")]
	public class V_1_1_0_14_DAO : DAO<VersionEntity>, IEssVersion
	{
		#region Ctor

		public V_1_1_0_14_DAO()
		{
		}

		#endregion

		#region Methods

		public void Excute()
		{
			DbExcutor.ExcuteNonQuery(@"UPDATE [dbo].[TNC_RateType] SET [Description] = 'SORA' WHERE SysKey = '2017042514462609136'", CommonConstant.SalesConnection);

			DbExcutor.ExcuteNonQuery(@"
			CREATE FUNCTION [dbo].[DownTreeLevel](@Id varchar(50))
			RETURNS @table TABLE(SPSK varchar(50),SPName varchar(150),ParentSK varchar(50),[Level] int, [Rank] int,GroupNum int)
			AS
			BEGIN
			DECLARE @temp TABLE(SPSK varchar(50),SPName varchar(150),ParentSK varchar(50),[Level] int, [Rank] int,GroupNum int)
			INSERT INTO @temp
			SELECT Person.SysKey,
			Person.Name,
			Employment.L1, 
			0,
			Employment.[Rank] , 
			0 FROM Person
			INNER JOIN Employment ON Person.SysKey = Employment.SysKey
			WHERE Person.TransType = 25 AND Person.IsActive = 1

			;WITH TREE (SPSK,SPName,ParentSK,[Level],[Rank],RowNum,GroupNum)
			AS
			(
				SELECT SPSK,
				SPName,
				ParentSK,
				1 [Level],
				[RANK],
				CONVERT(varchar(MAX),RIGHT(ROW_NUMBER() OVER (ORDER BY SPSK),10)) [RowNum],
				CONVERT(varchar(MAX),RIGHT(ROW_NUMBER() OVER (ORDER BY SPSK),10)) [GroupNum]
				FROM @temp WHERE ParentSK = @Id

				UNION ALL
	
				SELECT [Source].SPSK , 
				[Source].SPName, 
				[Source].ParentSK,
				TREE.Level + 1 ,
				[Source].[Rank],
				RowNum + convert(varchar(max),right(row_number() over (order by Tree.SPSK),10)),
				TREE.GroupNum
				FROM @temp [Source]
				INNER JOIN TREE ON TREE.SPSK = [Source].ParentSK
			)

			INSERT INTO @table
			SELECT SPSK,SPName,ParentSK,[Level],[Rank],GroupNum FROM TREE ORDER BY RowNum

			RETURN;

			END", CommonConstant.SalesConnection);
		}

		#endregion
	}
}

