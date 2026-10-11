namespace FC.Core.Data
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Ninject;
    using FC.Core.Infrastructure;
    using FC.Core.Data.Infrastructure.Implementation;
    using FC.Core.Data.Extensions;
 
    public class DbExcutor<TEntity> where TEntity : IEntity
    {
        #region Fields

        private DbConnection _connection;
        private DbCommand _command;
        private TEntity _entity;

        #endregion

        #region Ctor

        public DbExcutor() : this(Activator.CreateInstance<TEntity>()) { }

        private DbExcutor(TEntity entity)
        {
            _entity = entity;

            if (DI.Current.Get<IEssDataRepository>().DbConnection[entity.GetMetadata().KEY].Provider == DbProvider.MSSQL)
                _connection = new SqlConnection();
            else
                _connection = new OleDbConnection();

            _connection.ConnectionString = DI.Current.Get<IEssDataRepository>().DbConnection[entity.GetMetadata().KEY].ConnectionString;
            _command = _connection.CreateCommand();
        }

        #endregion

        #region Methods

        /// <summary>
        /// To excute select statement.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> ExcuteReader(string query)
        {
            Guard.IsNotNullOrEmpty(query, "query");

            var param = FilterRegEx(ref query);
            var excutor = new DbExcutor<TEntity>();

            try
            {
                excutor._command.CommandText = query;
                excutor._command.Parameters.Clear();

                if (param != null)
                    excutor._command.Parameters.AddRange(param.ToArray());

                excutor._connection.Open();

                var result = excutor._command.ExecuteReader();

                return new DbReaderItrator<TEntity>(result).GetValues().AsEnumerable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                excutor._connection.Close();
            }
        }
        
        /// <summary>
        /// To excute select statement.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TEntity>> ExcuteReaderAsync(string query)
        {
            Guard.IsNotNullOrEmpty(query, "query");

            var param = FilterRegEx(ref query);
            var excutor = new DbExcutor<TEntity>();

            try
            {
                excutor._command.CommandText = query;
                excutor._command.Parameters.Clear();

                if (param != null)
                    excutor._command.Parameters.AddRange(param.ToArray());

                excutor._connection.Open();

                var result = excutor._command.ExecuteReaderAsync();

                return await Task.FromResult(new DbReaderItrator<TEntity>(result.Result).GetValuesAsync().Result.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                excutor._connection.Close();
            }
        }

        /// <summary>
        /// To excute insert, update and delete.
        /// </summary>
        /// <param name="query">Query statement.</param>
        /// <returns></returns>
        /// 
        public static TEntity ExcuteNonQuery(string query)
        {
            Guard.IsNotNullOrEmpty(query, "query");
            var param = FilterRegEx(ref query);
            var excutor = new DbExcutor<TEntity>();

            try
            {
                excutor._command.CommandText = query;
                excutor._command.Parameters.Clear();

                if (param != null)
                    excutor._command.Parameters.AddRange(param.ToArray());

                excutor._connection.Open();
                excutor._command.ExecuteNonQuery();

                BindEntity(query,
                    excutor._command.Parameters.ToDictionary(),
                    ref excutor._entity);

                return excutor._entity;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                excutor._connection.Close();
            }
        }

        /// <summary>
        /// To excute insert, update and delete.
        /// </summary>
        /// <param name="query">Query statement.</param>
        /// <returns></returns>
        public async static Task<TEntity> ExcuteNonQueryAsync(string query)
        {
            Guard.IsNotNullOrEmpty(query, "query");
            var param = FilterRegEx(ref query);
            var excutor = new DbExcutor<TEntity>();

            try
            {
                excutor._command.CommandText = query;
                excutor._command.Parameters.Clear();

                if (param != null)
                    excutor._command.Parameters.AddRange(param.ToArray());

                excutor._connection.Open();

                var result = await excutor._command.ExecuteNonQueryAsync();

                BindEntity(query, excutor._command.Parameters.ToDictionary(), ref excutor._entity);

                return excutor._entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                excutor._connection.Close();
            }
        }

        /// <summary>
        /// To excute select statement.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static object ExcuteScalar(string query)
        {
            Guard.IsNotNullOrEmpty(query, "query");

            var param = FilterRegEx(ref query);
            var excutor = new DbExcutor<TEntity>();
            try
            {
                excutor._command.CommandText = query;
                excutor._command.Parameters.Clear();

                if (param != null)
                    excutor._command.Parameters.AddRange(param.ToArray());

                excutor._connection.Open();

                return excutor._command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                excutor._connection.Close();
            }
        }

        /// <summary>
        /// To excute select statement.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async static Task<TValue> ExcuteScalarAsync<TValue>(string query) where TValue : struct
        {
            Guard.IsNotNullOrEmpty(query, "query");

            var param = FilterRegEx(ref query);
            var excutor = new DbExcutor<TEntity>();
            try
            {
                excutor._command.CommandText = query;
                excutor._command.Parameters.Clear();

                if (param != null)
                    excutor._command.Parameters.AddRange(param.ToArray());

                excutor._connection.Open();

                return await Task.FromResult((TValue)excutor._command.ExecuteScalarAsync().Result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                excutor._connection.Close();
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// To remove and replace database parameters expression like <##> with actual @parameters symbol. 
        /// </summary>
        private static List<DbParameter> FilterRegEx(ref string sql)
        {
            //sql = sql.Replace("<#  #>", GeneralUtility.NullConstant);
            var provider = Activator.CreateInstance<TEntity>().GetMetadata().Provider;
            var parameter = new List<DbParameter>();
            var m = Regex.Match(sql, GeneralUtility.ParamRegEx);

            while (m.Success)
            {
                var key = "@" + Guid.NewGuid().ToString().Replace("-", "_");
             
                if (m.Groups["param"].ToString().Trim() == GeneralUtility.NullConstant)
                {
                    switch (provider)
                    {
                        case DbProvider.MSSQL:
                            parameter.Add(new SqlParameter { ParameterName = key, Value = DBNull.Value });
                            break;
                        case DbProvider.MySQL:
                            parameter.Add(new OleDbParameter { ParameterName = key, Value = DBNull.Value });
                            break;
                    }
                }
                else
                {
                    switch (provider)
                    {
                        case DbProvider.MSSQL:
                            parameter.Add(new SqlParameter { ParameterName = key, Value = m.Groups["param"].ToString().Trim() });
                            break;
                        case DbProvider.MySQL:
                            parameter.Add(new OleDbParameter { ParameterName = key, Value = m.Groups["param"].ToString().Trim() });
                            break;
                    }
                }

                sql = sql.Replace(m.Value, key);
                m = m.NextMatch();
            }
            return parameter.ConvertToLetterCaseOnDemand().ToList();
        }

        /// <summary>
        /// To convert to Db annotation symbol and make replacement in parameter value like <##>.
        /// </summary>
        private static Func<string, List<string>> FillToFunc = (x) =>
        {
            var newList = new List<string>();
            var regexObj = new Regex(GeneralUtility.CommaRegEx);
            var matchResults = regexObj.Match(x);

            while (matchResults.Success)
            {
                if (!string.IsNullOrEmpty(matchResults.Value))
                    newList.Add(matchResults.Value);

                matchResults = matchResults.NextMatch();
            }
            return newList;
        };

        private static void BindEntity(string query, IDictionary<string, object> dic, ref TEntity entity)
        {
            string json = "";
            if (query.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
            {
                var splitPart = Regex.Split(query, @"\s+").ToList();
                var fields = FillToFunc(splitPart[2].Replace('(', ',').Replace(')', ','));
                var parameters = FillToFunc(splitPart[4].Replace('(', ',').Replace(')', ','));
                var keyCollection = typeof(TEntity).GetRuntimeProperties().Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x));
                var intercept = keyCollection.Intersect(fields);

                foreach(var fieldName in intercept)
                {
                    var key = parameters[fields.IndexOf(fieldName) - 1];
                    if (dic.Keys.Contains(key))
                    {
                        if (dic[key] == DBNull.Value)
                        {
                            json += $" \"{ fieldName }\": null,";
                        }
                        else
                        {
                            json += $" \"{ fieldName }\":\"{DbParameterAnnotation.ConverTo(dic[key]).Replace("\"", string.Empty).Replace("<#", string.Empty).Replace("#>", string.Empty).Trim()}\"" + ",";
                        }
                    }
                }

                entity = Newtonsoft.Json.JsonConvert.DeserializeObject<TEntity>(@"{" + json + "}");
            }
        }

        #endregion
    }
    
    public class DbExcutor
    {
        #region Methods

        public static void ExcuteProc(string procedure, string connection)
        {
            try
            {
                ExcuteProc(procedure, connection, null);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static void ExcuteProc(string procedure, string connection, params DbParameter[] parameters)
        {
            Guard.IsNotNullOrEmpty(procedure, "procedure");
            Guard.IsNotNullOrEmpty(connection, "connection");

            try
            {
                using (var conn = new SqlConnection(DI.Current.Get<IEssDataRepository>().DbConnection[connection].ConnectionString))
                {
                    var command = conn.CreateCommand();

                    command.Parameters.Clear();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedure;

                    if (parameters != null && parameters.Count() > 0)
                        command.Parameters.AddRange(parameters);

                    conn.Open();

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExcuteNonQuery(string query, string connection)
        {
            Guard.IsNotNullOrEmpty(query, "query");
            Guard.IsNotNullOrEmpty(connection, "connection");

            try
            {
                using (var conn = new SqlConnection(DI.Current.Get<IEssDataRepository>().DbConnection[connection].ConnectionString))
                {
                    var command = conn.CreateCommand();

                    command.Parameters.Clear();
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = query;
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Task ExcuteProcAsync(string procedure, string connection)
        {
            return ExcuteProcAsync(procedure, connection, null);
        }

        public static Task ExcuteProcAsync(string procedure, string connection, params DbParameter[] parameters)
        {
            Guard.IsNotNullOrEmpty(procedure, "procedure");
            Guard.IsNotNullOrEmpty(connection, "connection");

            try
            {
                using (var conn = new SqlConnection(DI.Current.Get<IEssDataRepository>().DbConnection[connection].ConnectionString))
                {
                    var command = conn.CreateCommand();

                    command.Parameters.Clear();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = procedure;

                    if (parameters.Count() > 0)
                        command.Parameters.AddRange(parameters);

                    conn.Open();

                    return Task.FromResult(command.ExecuteNonQueryAsync().Result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public static object ExcuteScalar(string query,
            string connection)
        {
            Guard.IsNotNullOrEmpty(query, "query");
            Guard.IsNotNullOrEmpty(connection, "connection");

            try
            {
                var param = FilterRegEx(ref query);

                using (var conn = new SqlConnection(DI.Current.Get<IEssDataRepository>().DbConnection[connection].ConnectionString))
                {
                    var command = conn.CreateCommand();

                    command.Parameters.Clear();
                    command.CommandType = CommandType.Text;
                    command.CommandText = query;

                    if (param.Count() > 0)
                        command.Parameters.AddRange(param.ToArray());

                    conn.Open();

                    var value = command.ExecuteScalar();

                    if (value == DBNull.Value)
                        return null;

                    return value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable ExcuteDataTable(string query,
            string connection,
            string name,
            CommandType commendType = CommandType.Text)
        {
            Guard.IsNotNullOrEmpty(query, "query");
            Guard.IsNotNullOrEmpty(connection, "connection");
            Guard.IsNotNullOrEmpty(name, "query");

            var dataTable = new DataTable(name);

            try
            {
                using (var conn = new SqlConnection(DI.Current.Get<IEssDataRepository>().DbConnection[connection].ConnectionString))
                {
                    var param = FilterRegEx(ref query);
                    var command = conn.CreateCommand();

                    command.Parameters.Clear();
                    command.CommandType = commendType;
                    command.CommandText = query;

                    if (param.Count() > 0)
                        command.Parameters.AddRange(param.ToArray());

                    conn.Open();

                    var adapter = new SqlDataAdapter(command);

                    adapter.Fill(dataTable);
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Utilities

        private static List<DbParameter> FilterRegEx(ref string sql)
        {
            var parameter = new List<DbParameter>();
            var m = Regex.Match(sql, GeneralUtility.ParamRegEx);

            while (m.Success)
            {
                var key = "@" + Guid.NewGuid().ToString().Replace("-", "_");

                if (m.Groups["param"].ToString().Trim() == GeneralUtility.NullConstant)
                    parameter.Add(new SqlParameter { ParameterName = key, Value = DBNull.Value });
                else
                    parameter.Add(new SqlParameter { ParameterName = key, Value = m.Groups["param"].ToString().Trim() });

                sql = sql.Replace(m.Value, key);
                m = m.NextMatch();
            }
            return parameter.ConvertToLetterCaseOnDemand().ToList();
        }

        #endregion
    }
}
