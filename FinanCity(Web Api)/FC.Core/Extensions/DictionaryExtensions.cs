namespace FC.Core
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Data.OleDb;
    using System.Collections.Generic;
    using FC.Core.Infrastructure;
    using Ninject;

    public static class DictionaryExtensions
    {
        public static DbParameter[] ToDbParameters(this IDictionary<string, object> source, string connection)
        {
            Guard.IsNotNullOrEmpty(connection, "connection");

            var dictionary = DI.Current.Get<IEssDataRepository>().DbConnection;

            Guard.MustKeyContains(dictionary, connection);

            var parameters = new List<DbParameter>();

            var provider = dictionary[connection].Provider;

            if (provider == DbProvider.MSSQL)
            {
                source.Each(x =>
                {
                    parameters.Add(new SqlParameter { ParameterName = "@" + x.Key, Value = x.Value });
                });

                return parameters.ToArray();
            }
            else if (provider == DbProvider.MySQL)
            {
                source.Each(x =>
                {
                    parameters.Add(new OleDbParameter { ParameterName = "@" + x.Key, Value = x.Value });
                });

                return parameters.ToArray();
            }

            throw new NotSupportedException();
        }
    }
}
