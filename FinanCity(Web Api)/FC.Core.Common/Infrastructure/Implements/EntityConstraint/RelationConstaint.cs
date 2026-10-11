namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using FC.Core.Infrastructure;
   
    public class RelationConstaint : IReferenceFieldAnalyzer
    {
        #region Fields

        private IList<RelationContext> _depends;

        #endregion

        #region Ctor

        public RelationConstaint()
        {
            _depends = new List<RelationContext>();
        }

        #endregion

        #region Methods

        public void Register<TEntity>(Expression<Func<TEntity, object>> depends) where TEntity : IEntity
        {
            Guard.IsNotNull(depends, "reference");
            var member = depends.ToMemberExpression().Member;

            _depends.Add(new RelationContext
            {
                Table = typeof(TEntity).GetTableAttribute().Name,
                Column = ((PropertyInfo)member).GetColumnAttribute().Name ?? member.Name,
            });
        }

        public IList<RelationContext> Fields
        {
            get
            {
                return _depends;
            }
        }

        public void ValidateOnDelete<TEntity>(string table, string where) where TEntity : IEntity
        {
            _depends.Each(d =>
            {
                var instance = Activator.CreateInstance<TEntity>();

                var t = instance.GetType()
                 .GetCustomAttribute<DescriptionAttribute>() != null ? instance.GetType()
                 .GetCustomAttribute<DescriptionAttribute>().Description : instance.GetType().GetTableAttribute().Name;

                var query = $"SELECT SysKey FROM {d.Table} WHERE {d.Column} IN (SELECT SysKey FROM {table} WHERE {where})";
                var obj = DbExcutor<TEntity>.ExcuteReader(query);

                if (obj.Count() > 0)
                    throw new System.Exception($@"The regarding '{t}' 
                        has used in one or more other transaction and it could not be deleted.");
            });
        }

        #endregion
    }
}
