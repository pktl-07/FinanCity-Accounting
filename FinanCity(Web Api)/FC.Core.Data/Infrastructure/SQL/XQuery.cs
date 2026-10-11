namespace FC.Core.Data.SQL
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core.Data.SQL.Fluent;
    using FC.Core.Data.Infrastructure;

    public class XQuery : IHideObjectMembers
    {
        public static IColumn Select<T>(Action<IColumnFactory<T>> columns) where T : IEntity
        {
            return SelectBuilder.Create(Register(() => DependencyContainer.Current.Get<Select>())).Select(columns);
        }
        
        public static IColumn Select<T>(Expression<Func<T, dynamic>> selector) where T : IEntity
        {
            return SelectBuilder.Create(Register(() => DependencyContainer.Current.Get<Select>())).Select(selector);
        }

        public static IColumn Select<T>(Expression<Func<T, dynamic>> selector, Action<IColumnFactory<T>> columns) where T : IEntity
        {
            return SelectBuilder.Create(Register(() => DependencyContainer.Current.Get<Select>())).Select(selector, columns);
        }

        public static IInsertTable Insert<T>(Expression<Func<T, T>> fields) where T : IEntity
        {
            return InsertBuilder.Create(Register(() => DependencyContainer.Current.Get<Insert>())).Insert<T>(fields);
        }

        public static ICommitableTable Update<T>(Expression<Func<T, T>> fields) where T : IEntity
        {
            return UpdateBuilder.Create(Register(() => DependencyContainer.Current.Get<Update>())).Update<T>(fields);
        }
        
        public static ICommitableTable Delete<T>() where T : IEntity
        {
            return DeleteBuilder.Create(Register(() => DependencyContainer.Current.Get<Delete>())).Delete<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static TQueryComponent Register<TQueryComponent>(Func<TQueryComponent> factory)
            where TQueryComponent : QueryComponentBase
        {
            var component = factory();
            return component;
        }
    }
}
