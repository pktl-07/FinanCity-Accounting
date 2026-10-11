namespace FC.Core.Data.Infrastructure
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Base query component.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <typeparam name="TBuilder"></typeparam>
    public abstract class QueryComponent<TComponent, TBuilder>
       where TComponent : class
       where TBuilder : class
    {
        private static readonly Func<TComponent, TBuilder> creator = GetCreator();

        public static TBuilder Create(TComponent component)
        {
            return creator(component);
        }

        /// <summary>
        /// Runtime builder producer.
        /// </summary>
        /// <returns></returns>
        private static Func<TComponent, TBuilder> GetCreator()
        {
            var componentType = typeof(TComponent);
            var targetType = typeof(TBuilder);

            var argumentExpression = Expression.Parameter(componentType, "component");
            var constructor = targetType.GetConstructor(new Type[] { componentType });
            var newExpression = Expression.New(constructor, argumentExpression);

            return Expression.Lambda<Func<TComponent, TBuilder>>(newExpression, argumentExpression).Compile();
        }
    }
}
