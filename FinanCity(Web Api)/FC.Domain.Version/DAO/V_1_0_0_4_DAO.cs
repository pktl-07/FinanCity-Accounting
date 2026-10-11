namespace FC.Domain.Version.DAO
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using FC.Core;
	using FC.Core.Data;
	using FC.Domain.Version.Entity;
	using FC.Domain.Version.Infrastructure;
    
    [ESSVersion(Name = "1.0.0.4", Description = @"Addind new fileds for sales appointment.
	Alter System time format INT to VARCHAR(6) for all tables.")]
	public class V_1_0_0_4_DAO : DAO<VersionEntity>, IEssVersion
	{
		public void Excute()
		{
			DbExcutor.ExcuteNonQuery(@"CREATE TABLE [dbo].[JUN014](
					[SysKey] [varchar](50) PRIMARY KEY NOT NULL,
					[CreatedDate] [int] NULL,
					[CreatedTime] [varchar](6) NULL,
					[ModifiedDate] [int] NULL,
					[RecordStatus] [int] NOT NULL,
					[ModifiedUser] [varchar](150) NULL,
					[ModifiedTime] [varchar](6) NULL,
					[LoginUser] [varchar](150) NOT NULL,
					[Auto] [int] IDENTITY(300000,1) NOT NULL,
					[R1] [varchar](50) NULL,
					[VoucherID] [int] NULL,
					[Prefix] [varchar](50) NULL,
					[Postfix] [varchar](50) NULL,
					[Notify] [bit] NOT NULL,
					[StartDate] [int] NOT NULL,
					[StartTime] [varchar](6) NULL,
					[NotifyMeBeforeStartDate] [int] NOT NULL,
					[Description] [varchar](250) NULL,
					[RefSK] [varchar](50) NULL,
				)", CommonConstant.SalesConnection);

			var lib = new Dictionary<string, string>
			{
				["FC.Domain.Accounts"] = CommonConstant.AccountConnection,
				["FC.Domain.Sales"] = CommonConstant.SalesConnection,
				["FC.Domain.Auth"] = CommonConstant.AuthConnection
			};

			lib.Each(asm =>
		   {
			   Assembly.Load(asm.Key).GetTypes()
			   .Where(x => x.IsClass && x.BaseType == typeof(EntityBase))
			   .Each(type =>
			   {
				   var table = type.GetCustomAttribute<TableAttribute>()?.Name;
				   if (table != null)
				   {
					   DbExcutor.ExcuteNonQuery($@"ALTER TABLE {table} 
					   ALTER COLUMN CreatedTime varchar(6)", asm.Value);

					   DbExcutor.ExcuteNonQuery($@"ALTER TABLE {table} 
					   ALTER COLUMN ModifiedTime varchar(6)", asm.Value);
				   }
               });
		   });
		}
	}
}
