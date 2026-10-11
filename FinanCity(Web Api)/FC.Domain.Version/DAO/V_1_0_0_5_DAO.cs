namespace FC.Domain.Version.DAO
{
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;

	[ESSVersion(Name = "1.0.0.5", Description = @"")]
	public class V_1_0_0_5_DAO : DAO<VersionEntity>, IEssVersion
	{
		public void Excute()
		{
			for (int i = 1; i <= 8; i++)
			{
				DbExcutor.ExcuteNonQuery($@"ALTER TABLE B000000{i} 
				ALTER COLUMN CreatedTime varchar(6)",
				CommonConstant.AccountConnection);

				DbExcutor.ExcuteNonQuery($@"ALTER TABLE B000000{i} 
				ALTER COLUMN ModifiedTime varchar(6)",
				CommonConstant.AccountConnection);

				DbExcutor.ExcuteNonQuery($@"UPDATE B000000{i} SET CreatedTime='091055',
				ModifiedTime = NULL",
				CommonConstant.AccountConnection);
			}

			DbExcutor.ExcuteNonQuery(@"
				ALTER FUNCTION [dbo].[ConvertCustomTimeFormat](@Time VARCHAR(6))
                RETURNS VARCHAR(15)
                BEGIN
	
	                IF @Time IS NULL
	                BEGIN
		                RETURN NULL;
	                END 

	                IF (LEN(@Time) = 5)
	                 BEGIN
		                SELECT @Time = '0' + @Time
	                END

	                DECLARE @hour VARCHAR(2) = (SELECT SUBSTRING(@Time,1,2));
   	                DECLARE	@min VARCHAR(2) = (SELECT SUBSTRING(@Time,3,2));
	                DECLARE @second VARCHAR(2) = (SELECT SUBSTRING(@Time,5,2));
	                DECLARE @l_time VARCHAR(15) = @hour + ':' + @min + ':' + @second;

	                RETURN FORMAT(CAST(@l_time AS DATETIME), 'hh:mm:ss tt')
                END", CommonConstant.AccountConnection);
		}
	}
}
