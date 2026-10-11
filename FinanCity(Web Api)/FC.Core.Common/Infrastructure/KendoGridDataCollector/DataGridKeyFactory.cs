namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class DataGridKeyFactory<T>
    {
        internal List<string> Keys;
        public DataGridKeyFactory()
        {
            Keys = new List<string>();
        }

        public void Add<TValue>(Expression<Func<T, TValue>> expression)
        {
            var body = expression.Body as MemberExpression;

            Keys.Add(body.Member.Name);
        }
    }
}