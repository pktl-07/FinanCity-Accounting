namespace FC.Core.Infrastructure.Implements.RecordIndexer
{
    using System;
    using System.Linq;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using FC.Core;
    using FC.Core.Data;

    public class ManualSource : RecordIndexerBaseStore
    {
        #region Fields

        private IEntity _sourceEntity;
        private TransType _transType;
        private RecordIndexerEntity _info;

        #endregion

        #region Ctor

        public ManualSource(TransType trasnType, IEntity entity, RecordIndexerEntity info)
          : base(trasnType, entity, info)
        {
            _transType = trasnType;
            _sourceEntity = entity;
            _info = info;
        }

        #endregion

        #region Methods

        public override string LastR1
        {
            get
            {
                var connection = new SqlConnection(_sourceEntity.GetMetadata().ConnectionString);
                var command = new SqlCommand();
                var query = @"SELECT R1 FROM " + TableName + " WHERE Auto = (SELECT ISNULL(MAX(Auto),0) FROM " + TableName + ")";

                if (_sourceEntity.GetType().GetProperty("TransType") != null)
                {
                    query += " AND " + "TransType=@TransType";

                    command.Parameters.Add(new SqlParameter { ParameterName = "@TransType", Value = _info.TransType });
                }

                command.Connection = connection;
                command.CommandText = query;

                try
                {
                    connection.Open();
                    var value = command.ExecuteScalar();

                    if (value == DBNull.Value)
                        return _info.Prefix + "0" + _info.Postfix;
                    else
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
        }

        public override RecordIndexrContext GetResult()
        {
            var manualId = LastR1;

            if (manualId == null)
            {
                return new RecordIndexrContext
                {
                    Prefix = _info.Prefix,
                    R1 = _info.Prefix + "1" + _info.Postfix,
                    Postfix = _info.Postfix,
                    VoucherId = 1,
                    Format = _info.Format
                };
            }
            else
            {
                var vourcherSettings = FilterInt(manualId);
                var lastId = int.Parse(vourcherSettings[1]) + 1;

                return new RecordIndexrContext
                {
                    Prefix = vourcherSettings[0],
                    R1 = vourcherSettings[0] + lastId + vourcherSettings[2],
                    VoucherId = lastId,
                    Postfix = vourcherSettings[2],
                    Format = _info.Format,
                };
            }
        }

        #endregion

        #region Utilities

        private string[] FilterInt(string value)
        {

            var number = 0;
            var prefix = "";
            var postfix = "";
            var digitCount = Regex.Split(value, @"\D+");

            foreach (char v in value.ToArray())//TO FIND PREFIX
            {
                if (int.TryParse(v.ToString(), out number))
                    break;

                prefix += v.ToString();
            }

            foreach (string digit in digitCount) // TO FIND ID NUMBER
            {
                if (int.TryParse(digit, out number))
                {
                    number = int.Parse(digit);

                    break;
                }
            }

            //TO FIND POST FIX
            var combineString = prefix + number.ToString();
            var length = combineString.Length;

            try
            {
                postfix = value.Substring(length);
            }
            catch
            {
                postfix = "";
            }
            return new string[] { prefix, number.ToString(), postfix };
        }


        #endregion
    }
}
