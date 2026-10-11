namespace FC.Core.Infrastructure.Implements.DataCollector
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Core.Common.Extensions;

    public class EntityAnalyzer<T> : IEntityAnalyzer<T> where T : class
    {
        #region Fields

        private IDataKeyConfigurator<T> _selectorKey;
        private Action<IDataKeyConfigurator<T>> _config;
        private IEnumerable<T> _origin;
        private IEnumerable<T> _compare;

        #endregion

        #region Ctor

        public EntityAnalyzer(Action<IDataKeyConfigurator<T>> key, IEnumerable<T> origin, IEnumerable<T> compare)
        {
            _selectorKey = new DataKeyConfigurator<T>();
            _config = key;
            _origin = origin;
            _compare = compare;

            key(_selectorKey);
        }

        #endregion

        #region Methods

        public IEnumerable<T> InsertedSource(bool distinct = false)
        {
            if (distinct)
                return _compare.Except(_origin, this);

            return _compare.Except(_origin, this).Complete(_compare, _config);
        }

        public IEnumerable<T> UpdatedSource(bool distinct = false)
        {
            if (distinct)
                return _compare.Intersect(_origin, this);

            return _compare.Intersect(_origin, this).Complete(_compare, _config);
        }

        public IEnumerable<T> DeletedSource(bool distinct = false)
        {
            if (distinct)
                return _origin.Except(_compare, this);

            return _origin.Except(_compare, this).Complete(_origin, _config);
        }

        public bool Equals(T x, T y)
        {
            Expression body = null;//expression body
            var pramX = Expression.Parameter(typeof(T), "x");//parameter x
            var pramY = Expression.Parameter(typeof(T), "y");//parameter y

            for (int i = 0; i < _selectorKey.Keys.Count; i++)
            {
                var propName = (_selectorKey.Keys[i].Body as MemberExpression).Member.Name;//property name

                var memberX = Expression.Property(pramX, propName);//member x
                var memberY = Expression.Property(pramY, propName);//member y

                if (i == 0)
                    body = Expression.Equal(memberX, memberY);//x.SysKey==y.SysKey
                else
                    body = Expression.AndAlso(body, Expression.Equal(memberX, memberY));//combine && condition
            }

            return Expression.Lambda<Func<T, T, bool>>(body, pramX, pramY).Compile()(x, y);
        }

        public int GetHashCode(T obj)
        {
            Expression body = null;//expression body
            var pramX = Expression.Parameter(typeof(T), "x");//parameter x

            foreach (var expr in _selectorKey.Keys)
                if (expr.Compile()(obj) == null)//when null then return 0 to false condition
                    return 0;

            for (int i = 0; i < _selectorKey.Keys.Count; i++)
            {
                var propName = (_selectorKey.Keys[i].Body as MemberExpression).Member.Name;//extract property name
                var memberX = Expression.Property(pramX, propName);//eg. x.SysKey
                var method = Expression.Call(memberX, obj.GetType().GetMethod("GetHashCode"));//eg. x.SysKey.GetHashCode()

                if (i == 0)
                    body = method;
                else
                    body = Expression.ExclusiveOr(body, method);//eg. x.SysKey.GetHashCode() ^ x.NRIC.GetHashCode()
            }

            return Expression.Lambda<Func<T, int>>(body, pramX).Compile()(obj);
        }

        #endregion
    }
}
