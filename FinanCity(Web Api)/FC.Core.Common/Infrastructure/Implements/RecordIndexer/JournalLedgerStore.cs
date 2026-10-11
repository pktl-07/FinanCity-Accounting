namespace FC.Core.Infrastructure.Implements.RecordIndexer
{
    using System;
    using System.Data.SqlClient;
    using FC.Core;
    using FC.Core.Data;
    using Ninject;

    public class JournalLedgerStore : RecordIndexerBaseStore
    {
        #region Fields

        private IEntity _sourceEntity;
        private TransType _transType;
        private RecordIndexerEntity _info;
        private string GetStringFormat()
        {
            var result = "";
            for (int i = 0; i < _info.EndValue.ToString().Length; i++)
                result += "0";

            return result;
        }

        #endregion

        #region Ctor

        public JournalLedgerStore(TransType trasnType, IEntity entity, RecordIndexerEntity info)
           : base(trasnType, entity, info)
        {
            _transType = trasnType;
            _sourceEntity = entity;
            _info = info;
        }

        #endregion

        #region Methods

        public override int LastIndexingNumber
        {
            get
            {
                try
                {
                    using (var connection = new SqlConnection(DI.Current.Get<IEssDataRepository>().DbConnection["FC.Biz"].ConnectionString))
                    {
                        var command = connection.CreateCommand();
                        var query = @"INSERT INTO TransactionIdGenerator(Tbl) VALUES (@Table);
                            SELECT IDENT_CURRENT('TransactionIdGenerator')";

                        command.Parameters.Add(new SqlParameter { ParameterName = "@Table", Value = TableName });
                        command.Connection = connection;
                        command.CommandText = query;

                        connection.Open();

                        var obj = command.ExecuteScalar();
                        return int.Parse(obj.ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public override RecordIndexrContext GetResult()
        {
            var lastId = LastIndexingNumber;
            return new RecordIndexrContext
            {
                Prefix = _info.Prefix,
                R1 = _info.Prefix + lastId.ToString(GetStringFormat()) + _info.Postfix,
                VoucherId = lastId,
                Postfix = _info.Postfix,
                Format = 0,
            };
        }


        #endregion
    }
}
