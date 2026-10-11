namespace FC.Core.Infrastructure.Implements.RecordIndexer
{
    using System;
    using System.Data.SqlClient;
    using System.Reflection;
    using FC.Core;
    using FC.Core.Data;

    public class YearMonthSource : RecordIndexerBaseStore
    {
        #region Fields

        private IEntity _sourceEntity;
        private TransType _transType;
        private RecordIndexerEntity _info;

        #endregion

        #region Methods

        public YearMonthSource(TransType trasnType, IEntity entity, RecordIndexerEntity info)
           : base(trasnType, entity, info)
        {
            _transType = trasnType;
            _sourceEntity = entity;
            _info = info;
        }

        public override int LastIndexingNumber
        {
            get
            {
                var connection = new SqlConnection(_sourceEntity.GetMetadata().ConnectionString);
                var command = new SqlCommand();

                try
                {
                    var currentDate = GetDateDependency();
                    var currentYear = currentDate.ToString().Substring(2, 2);
                    var currentMonth = currentDate.ToString().Substring(4, 2);
                    var formatedYearMonth = currentYear + currentMonth;
                    var lenght = _info.EndValue.ToString().Length;
                    var shouldStartValue = _info.StartValue - 1;

                    var query = @"SELECT ISNULL(MAX(VoucherID)," + formatedYearMonth + shouldStartValue.ToString(StringFormat) + ")"
                        + "FROM " + TableName + " WHERE " + _info.BaseDate + " LIKE @DependencyDate"
                        + " AND VoucherID LIKE @VoucherID"
                        + " AND LEN(VoucherID)=@Length";

                    command.Parameters.Add(new SqlParameter { ParameterName = "@DependencyDate", Value = "%" + formatedYearMonth + "%" });

                    command.Parameters.Add(new SqlParameter { ParameterName = "@VoucherID", Value = "%" + formatedYearMonth + "%" });

                    command.Parameters.Add(new SqlParameter { ParameterName = "@Length", Value = lenght + formatedYearMonth.Length });

                    if (_sourceEntity.GetType().GetProperty("TransType") != null)
                    {
                        query += " AND TransType =@TransType";
                        command.Parameters.Add(new SqlParameter { ParameterName = "@TransType", Value = _sourceEntity.GetType().GetProperty("TransType").GetValue(_sourceEntity) ?? DBNull.Value });
                    }

                    command.Connection = connection;

                    command.CommandText = query;

                    connection.Open();

                    var result = command.ExecuteScalar();

                    if (result != null)
                        return (int)result;
                    else
                        return 0;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public override RecordIndexrContext GetResult()
        {
            try
            {
                var lastIndexingNumber = LastIndexingNumber + 1;

                return new RecordIndexrContext
                {
                    VoucherId = lastIndexingNumber,
                    Postfix = _info.Postfix,
                    Prefix = _info.Prefix,
                    R1 = _info.Prefix + lastIndexingNumber + _info.Postfix,
                    Format = _info.Format,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private string StringFormat
        {
            get
            {
                var lenght = _info.EndValue.ToString().Length;
                var stringFormat = "";
                for (int i = 0; i < lenght; i++)
                {
                    stringFormat += "0";
                }
                return stringFormat;
            }
        }

        private int GetDateDependency()
        {
            try
            {
                var property = _sourceEntity.GetType().GetProperty(_info.BaseDate);

                if (property.Name.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase))
                {
                    return GeneralUtility.ServerDate;
                }

                var value = (string)property.GetValue(_sourceEntity);

                var exceptionMessage = " is set as Indexing Date that can not be empty value";

                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(property.GetCustomAttribute<ColumnAttribute>().Name + exceptionMessage);
                }

                return int.Parse(value);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}
