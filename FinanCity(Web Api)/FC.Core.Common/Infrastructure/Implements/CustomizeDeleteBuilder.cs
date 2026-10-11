namespace FC.Core.Infrastructure.Implements
{
    using System;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core.Data.SQL;
    using FC.Core;
    using FC.Core.Data.Extensions;

    public class CustomizeDeleteBuilder : Delete
    {
        public CustomizeDeleteBuilder()
        {
        }

        public override void WriteWhere<T>(Expression expr)
        {
            base.WriteWhere<T>(expr);
            var w = base.Where;

            var instance = Activator.CreateInstance<T>();
            if (instance is IReferencesConstraint)
            {
                var r = DI.Current.Get<IReferenceFieldAnalyzer>();
                var t = typeof(T).GetTableAttribute().Name;

                (instance as IReferencesConstraint).RelationWith(r);
                r.ValidateOnDelete<T>(table: t, where: w);
            }
        }
    }
}
