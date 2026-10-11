namespace FC.Core.Infrastructure.Implements.RecordIndexer
{
    using System;
    using System.Data.SqlClient;
    using FC.Core.Exceptions;
    using FC.Core;
    using FC.Core.Data;
 
    public abstract class RecordIndexerBaseStore
    {
        #region Fields

        private TransType _transType;
        private IEntity _sourceEntity;
        private RecordIndexerEntity _info;

        #endregion

        #region Ctor

        public RecordIndexerBaseStore(TransType transType, IEntity source, RecordIndexerEntity info)
        {
            _transType = transType;
            _sourceEntity = source;
            _info = info;
        }

        #endregion

        #region Members

        public string TableName
        {
            get
            {
                if (_transType == TransType.Journal)
                {
                    var query = @"SELECT BatchTableName FROM ACC005 WHERE YC = (SELECT YC FROM ACC006 WHERE IsDefaultYear = 1 GROUP BY YC)  AND ModuleCode = 'GLC'";
                    var connection = new SqlConnection(_sourceEntity.GetMetadata().ConnectionString);
                    var command = new SqlCommand(query, connection);

                    try
                    {
                        connection.Open();
                        return (string)command.ExecuteScalar().ToReplaceDbNull();
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
                else
                {
                    return _transType.ToTableName();
                }
            }
        }

        #endregion

        #region Methods

        public virtual int LastIndexingNumber
        {
            get
            {
                var query = $@"SELECT ISNULL(MAX(VoucherID),0) FROM {TableName} WHERE 1=1 ";
                var connection = new SqlConnection(_sourceEntity.GetMetadata().ConnectionString);
                var command = new SqlCommand();

                if (!string.IsNullOrWhiteSpace(_info.Prefix))
                {
                    query += " AND Prefix=@Prefix";
                    command.Parameters.Add(new SqlParameter { ParameterName = "@Prefix", Value = (object)_info.Prefix ?? DBNull.Value });
                }

                if (!string.IsNullOrWhiteSpace(_info.Postfix))
                {
                    query += " AND Postfix=@Postfix";
                    command.Parameters.Add(new SqlParameter { ParameterName = "@Postfix", Value = (object)_info.Postfix ?? DBNull.Value });
                }

                if (_sourceEntity != null)
                {
                    if (_sourceEntity.GetType().GetProperty("TransType") != null)
                    {
                        query += " AND TransType=@TransType";
                        command.Parameters.Add(new SqlParameter { ParameterName = "@TransType", Value = (int)_transType });
                    }
                }

                command.Connection = connection;
                command.CommandText = query;

                try
                {
                    connection.Open();

                    var result = command.ExecuteScalar();

                    if (result != null)
                        return (int)result;

                    else
                        return _info.StartValue;
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

        public virtual string LastR1
        {
            get
            {
                var connection = new SqlConnection(_sourceEntity.GetMetadata().ConnectionString);
                var command = new SqlCommand();

                try
                {
                    var query = $@"SELECT R1 FROM {TableName} WHERE Auto = (SELECT ISNULL(MAX(Auto),0) FROM {TableName} WHERE 1=1 ";
                    if (!string.IsNullOrWhiteSpace(_info.Prefix))
                    {
                        query += " AND Prefix=@Prefix";

                        command.Parameters.Add(new SqlParameter { ParameterName = "@Prefix", Value = _info.Prefix });
                    }

                    if (!string.IsNullOrWhiteSpace(_info.Postfix))
                    {
                        query += " AND Postfix=@Postfix";

                        command.Parameters.Add(new SqlParameter { ParameterName = "@Postfix", Value = _info.Postfix });
                    }
                    query += ")";
                    command.Connection = connection;
                    command.CommandText = query;
                    connection.Open();

                    return (string)command.ExecuteScalar().ToReplaceDbNull() ?? _info.Prefix + _info.StartValue + _info.Postfix;
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

        public virtual RecordIndexrContext GetResult()
        {
            try
            {
                var lastIndexingNumber = LastIndexingNumber < _info.StartValue ? _info.StartValue : LastIndexingNumber + 1;
                var maxlength = _info.EndValue.ToString().Length;
                var lastIdLength = lastIndexingNumber.ToString().Length;
                var diffLenght = 0;
                var strhelper = "";

                if (lastIndexingNumber > _info.EndValue)
                {
                    throw new VoucherNumberExceedException();
                }

                lastIndexingNumber = lastIndexingNumber == 1 ? _info.StartValue : lastIndexingNumber;
                lastIdLength = lastIndexingNumber.ToString().Length;
                if (maxlength > lastIdLength)
                {
                    diffLenght = maxlength - lastIdLength;
                    for (int i = 0; i < diffLenght; i++)
                    {
                        strhelper += "0";
                    }
                }

                return new RecordIndexrContext
                {
                    VoucherId = lastIndexingNumber,
                    Postfix = _info.Postfix,
                    Prefix = _info.Prefix,
                    R1 = _info.Prefix + strhelper + lastIndexingNumber + _info.Postfix,
                    Format = _info.Format,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
