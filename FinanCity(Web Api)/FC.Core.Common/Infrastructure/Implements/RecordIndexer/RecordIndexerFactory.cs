namespace FC.Core.Infrastructure.Implements.RecordIndexer
{
    using System;
    using System.Data.SqlClient;
    using Ninject;
    using FC.Core;
    using FC.Core.Data;

    public class RecordIndexerFactory
    {
        #region Fields

        private RecordIndexerBaseStore _indexingBaseContext;

        #endregion

        #region Utilities

        private RecordIndexerEntity GetRecordIndexerInfo(TransType transType)
        {
            var result = new RecordIndexerEntity();
            var commend = new SqlCommand();
            var connection = new SqlConnection(DI.Current.Get<IEssDataRepository>().DbConnection["FC.Biz"].ConnectionString);

            try
            {
                var query = $@"SELECT * FROM IDIndexing WHERE TransType={(int)transType}";

                commend.CommandText = query;
                commend.Connection = connection;
                connection.Open();
                var reader = commend.ExecuteReader();

                while (reader.Read())
                {
                    result.Prefix = (string)reader["Prefix"].ToReplaceDbNull();
                    result.Postfix = (string)reader["Postfix"].ToReplaceDbNull();
                    result.StartValue = (int)reader["StartValue"].ToReplaceDbNull();
                    result.EndValue = (int)reader["EndValue"].ToReplaceDbNull();
                    result.TransType = (int)reader["TransType"].ToReplaceDbNull();
                    result.Format = (int)reader["Format"].ToReplaceDbNull();
                    result.BaseDate = (string)reader["BaseDate"].ToReplaceDbNull();
                    result.Description = (string)reader["Description"].ToReplaceDbNull();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        #endregion

        #region Methods

        public RecordIndexrContext Generate(TransType transType,
           IEntity businessEntity)
        {
            try
            {
                var info = GetRecordIndexerInfo(transType);

                if (transType.ToModuleType() == ModuleType.GL && info.Format == (int)IndexingFormat.BuildIn)
                    _indexingBaseContext = new JournalLedgerStore(transType, businessEntity, info);
                else
                {
                    if (info.Format == (int)IndexingFormat.Manual)
                        _indexingBaseContext = new ManualSource(transType, businessEntity, info);

                    else if (info.Format == (int)IndexingFormat.BaseOnCurrentDate)
                        _indexingBaseContext = new YearMonthSource(transType, businessEntity, info);
                    else
                        _indexingBaseContext = new DefaultDataStore(transType, businessEntity, info);
                }
                return _indexingBaseContext.GetResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
