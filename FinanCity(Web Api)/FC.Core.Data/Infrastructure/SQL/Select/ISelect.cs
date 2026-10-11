namespace FC.Core.Data.SQL
{
    using System;
    using System.Linq.Expressions;
    using FC.Core.Data.SQL.Fluent;

    public interface ISelect 
    {
        IColumn Select<T>(Action<ColumnFacotry<T>> columns) where T : IEntity;

        IColumn Select<T>(Expression<Func<T, dynamic>> predicate) where T : IEntity;

        IColumn Select<T>(Expression<Func<T, dynamic>> predicate, Action<ColumnFacotry<T>> columns) where T : IEntity;
    }
}
