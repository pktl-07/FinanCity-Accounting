namespace FC.Core.Data.Infrastructure.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Ninject;
    using Extensions;

    /// <summary>Object mapping to produce IEnumerable list</summary>
    /// <remarks>It seems more effective way of loading data on the object per request.</remarks>
    /// <typeparam name="T"></typeparam>
    internal class DbReaderItrator<T> /*: IEnumerable<T> where T : IEntity*/
        where T : IEntity
    {
        #region Fields

        private T _type;
        private DbDataReader _reader;
        private Hashtable _hashtable;

        #endregion

        #region Ctor

        public DbReaderItrator(DbDataReader reader)
        {
            _reader = reader;
            _hashtable = new Hashtable();

            typeof(T).GetProperties().Each(x =>
            {
                var col = x.Name.ToUpper();
                _hashtable[col] = x;
            });
        }

        #endregion

        #region  Methods

        public IList<T> GetValues()
        {
            var rows = new List<T>();

            while (_reader.Read())
                rows.Add(BindAnyType());

            return rows;
        }

        public async Task<IList<T>> GetValuesAsync()
        {
            var rows = new List<T>();

            while (await _reader.ReadAsync())
                rows.Add(BindAnyType());

            return rows;
        }

        #endregion

        #region Utilities

        private T BindAnyType()
        {
            try
            {
                _type = Activator.CreateInstance<T>();
                for (int _index = 0; _index < _reader.FieldCount; _index++)
                {
                    var info = (PropertyInfo)_hashtable[_reader.GetName(_index).ToUpper()];
                    if ((info != null) && info.CanWrite)
                    {
                        var targetType = info.PropertyType.IsNullableType() ? Nullable.GetUnderlyingType(info.PropertyType) : info.PropertyType;
                        var convertedValue = _reader.GetValue(_index) == DBNull.Value ? null : ConvertValue(targetType.FullName, _reader.GetValue(_index).ToString());

                        if (convertedValue is string)
                        {
                            if (!info.Name.Equals("BaseDate", StringComparison.OrdinalIgnoreCase))
                                if (DependencyContainer.Current.Get<IEssAppRepository>().LetterFormat == LetterCasing.Upper)
                                    convertedValue = convertedValue.ToString().Trim().ToUpper();
                                else if (DependencyContainer.Current.Get<IEssAppRepository>().LetterFormat == LetterCasing.Lower)
                                    convertedValue = convertedValue.ToString().Trim().ToLower();
                                else
                                {
                                    convertedValue = convertedValue.ToString().Trim();
                                }
                        }

                        info.SetValue(_type, convertedValue, null);
                    }
                }
                return _type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private object ConvertValue(string typeInString, string value)
        {
            Type originalType = Type.GetType(typeInString);

            var underlyingType = Nullable.GetUnderlyingType(originalType);

            // if underlyingType has null value, it means the original type wasn't nullable
            object instance = Convert.ChangeType(value, underlyingType ?? originalType);

            return instance;
        }

        #endregion
    }
}

