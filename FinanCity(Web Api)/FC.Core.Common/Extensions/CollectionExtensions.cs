namespace FC.Core.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Infrastructure.Implements.DataCollector;

    public static class CollectionExtensions
    {
        #region Methods

        /// <summary>
        /// Especially to use in linq collection extension such as Intersect, Except, Union to fill the full complement
        /// Because the System.Linq collection distinct the array when make Intersect, Except or Union ect... which sometime outcome undesire result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source1">Source of array after Linq.Except, Intersect or Union</param>
        /// <param name="source2">Source of origin array to pick upSear</param>
        /// <param name="key">Selector key</param>
        /// <returns>New full complement enumerable</returns>
        public static IEnumerable<T> Complete<T>(this IEnumerable<T> source1, IEnumerable<T> source2, Action<IDataKeyConfigurator<T>> key) where T : class
        {
            var result = new List<T>();

            source1.Each(d =>
            {
                result.AddRange(source2.Where(LogicalEquals(d, key)));
            });

            return result.AsEnumerable();
        }
        
        /// <summary>
        /// To convert expression
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Expression ToMemberInitExpression<T>(this IDictionary<string,object> source)
        {
            var members = new List<MemberAssignment>();

            foreach(var dic in source)
            {
                var propertyType = typeof(T).GetProperty(dic.Key).PropertyType;

                members.Add(Expression.Bind(typeof(T).GetProperty(dic.Key), Expression.Constant(dic.Value, propertyType)));
            }

            var body = Expression.MemberInit(Expression.New(typeof(T)), members);

            return Expression.Lambda<Func<T, T>>(body, Expression.Parameter(typeof(T), "x"));
        }

        #endregion

        #region Utilities

        //eg. x=>x.SysKey == "123"
        private static Func<T, bool> LogicalEquals<T>(T compare, Action<IDataKeyConfigurator<T>> key) where T : class
        {
            //create new service instance
            //DI.Current.Bind<DataKeyConfigurator<T>>(); 
            //.AddTransient<DataKeyConfigurator<T>>();
            //call service instance to construct object this is the ame as new DataKeyConfigurator()
            //var keyselector = DI.Current.Get<DataKeyConfigurator<T>>(); // .BuildServiceProvider().GetService<DataKeyConfigurator<T>>();
            var keyselector = new DataKeyConfigurator<T>();

            key(keyselector);

            Expression body = null; // expression body
            var pramX = Expression.Parameter(typeof(T), "x");//parameter x

            for (int i = 0; i < keyselector.Keys.Count; i++)
            {
                var prop = (keyselector.Keys[i].Body as MemberExpression).Member.Name;//property name

                var memberX = Expression.Property(pramX, prop);//eg. x=>x.SysKey
                var constancExpression = Expression.Constant(keyselector.Keys[i].Compile()(compare));//eg. x=>x.SysKey = "123"

                if (i == 0)
                    body = Expression.Equal(memberX, constancExpression);//x.SysKey==y.SysKey
                else //unless more than one condition than build expression tree to become like x.SysKey == "123" && y.SysKey =="123" && x.NRIC == "123" && y.NRIC =="123"
                    body = Expression.AndAlso(body, Expression.Equal(memberX, constancExpression));//combine && condition
            }

            return Expression.Lambda<Func<T, bool>>(body, pramX).Compile();
        }

        #endregion
    }
}
