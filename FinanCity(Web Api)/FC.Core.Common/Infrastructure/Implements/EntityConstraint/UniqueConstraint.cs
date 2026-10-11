namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Core.Infrastructure;
    using FC.Core.Exceptions;
 
    public class UniqueConstraint<TEntity> 
        : IUniqueFieldAnalyzer<TEntity> where TEntity : IEntity
    {
        #region Fields

        private int _counter;
        private IList<PropertyInfo[]> _fields;
        private DBUniqueFieldsException _exception;
       
        #endregion

        #region Ctor

        public UniqueConstraint(DBUniqueFieldsException exception)
        {
            _fields = new List<PropertyInfo[]>();
            _counter = 0;
            _exception = exception;
        }

        #endregion

        #region Methods

        public void Register(Expression<Func<TEntity, object[]>> fields)
        {
            var columns = new List<PropertyInfo>();
            if (fields.Body is NewArrayExpression)
            {
                (fields.Body as NewArrayExpression)
                    .Expressions
                    .Each(arr =>
                {
                    var member = (arr as MemberExpression);
                    var column = (PropertyInfo)member.Member;
              
                    columns.Add(column);
                });
            }

            _fields.Add(columns.ToArray());
            _counter++;
        }

        public void ValidateOnSave(TEntity entity)
        {
            NotifyDependencyOnInsert(entity, (query, error) =>
            {
                var r = DbExcutor<TEntity>.ExcuteReader(query);
                if (r.Count() > 0)
                {
                    _exception.ErrorContext = error;
                    throw _exception;
                }
            });
        }

        public void ValidateOnUpdate<T>(IDictionary<string, object> fields,
            Expression<Func<T, bool>> filter) where T : IEntity
        {
            string key = null;

            filter.CreateObject((mem, value) =>
            {
                if (mem == "SysKey")
                    key = value;
            });

            Guard.IsNotNull(key, @"Table with unique constraint 
                should be included System Key inside filter.");

            NotifyDependencyOnUpdate<T>(fields.Select(x => new { x.Key, x.Value }),
                (query, error) =>
                {
                    var r = DbExcutor<TEntity>.ExcuteReader($"{query} AND SysKey <> <# {key} #>");
                    if (r.Count() > 0)
                    {
                        _exception.ErrorContext = error;
                        throw _exception;
                    }
                });
        }

        public void ValidateOnUpdate<T>(IDictionary<string, object> fields,
            string filter) where T : IEntity
        {
            NotifyDependencyOnUpdate<T>(
                fields.Select(x => new { x.Key, x.Value }),
                (query, error) =>
                {
                    var except = query.Split(new string[] { "WHERE" }, StringSplitOptions.None)[0];
                    var r = DbExcutor<TEntity>.ExcuteReader(
                        $@"
                        {query} 
                        EXCEPT 
                        {except}
                        WHERE {filter}");

                    if (r.Count() > 0)
                    {
                        _exception.ErrorContext = error;
                        throw _exception;
                    }
                });
        }

        #endregion

        #region Members

        public IList<PropertyInfo[]> Fields
        {
            get
            {
                return _fields;
            }
        }

        #endregion

        #region Utilities

        private void NotifyDependencyOnUpdate<T>(
            IEnumerable<dynamic> fields,
            Action<string, List<UniqueFieldError>> action)
        {
            var table = typeof(TEntity).GetTableAttribute().Name;
            var m = new List<UniqueFieldError>();

            _fields.Each(cols =>
            {
                var query = $"SELECT SysKey FROM {table} WHERE 1=1 ";
                cols.Each(col =>
                {
                    var c = col.GetColumnAttribute().Name ?? col.Name;
                    var data = fields.Where(x => x.Key == c).FirstOrDefault();
                   
                    if (data != null)
                    {
                        var d = col.GetCustomAttribute<DisplayAttribute>() != null
                            ? col.GetCustomAttribute<DisplayAttribute>().Name : c;

                        m.Add(new UniqueFieldError
                        {
                            Data = data.Value.ToString(),
                            Display = d
                        });

                        query += $@" AND {c}=<# {data.Value} #>";
                        
                        action(query, m); //notify
                    }
                });
            });
        }
        
        private void NotifyDependencyOnInsert(TEntity entity, 
            Action<string, List<UniqueFieldError>> action)
        {
            var table = typeof(TEntity).GetTableAttribute().Name;
            var m = new List<UniqueFieldError>();

            _fields.Each(cols =>
            {
                var query = $"SELECT SysKey FROM {table} WHERE 1=1 ";
                //construct query
                cols.Each(col =>
                {
                    var c = col.GetColumnAttribute().Name ?? col.Name;
                    var p = entity.GetType().GetProperty(c).GetValue(entity);
                    var d = col.GetCustomAttribute<DisplayAttribute>() != null
                    ? col.GetCustomAttribute<DisplayAttribute>().Name : c;
                        
                    m.Add(new UniqueFieldError
                    {
                        Data = p?.ToString(),
                        Display = d
                    });

                    query += $@" AND {c}=<# {p} #>";
                    action(query, m); //notify
                });
            });
        }

        #endregion
    }
}
