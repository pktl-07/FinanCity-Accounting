namespace FC.Core.Data.Infrastructure
{
    using FC.Core.Infrastructure;

    /// <summary>
    /// Component builder base.
    /// </summary>
    /// <typeparam name="TComponent">Type component.</typeparam>
    /// <typeparam name="TBuilder">Type builder</typeparam>
    public abstract class QueryComponentBuilderBase<TComponent, TBuilder> : QueryComponent<TComponent, TBuilder>
        where TComponent : QueryComponentBase
        where TBuilder : QueryComponentBuilderBase<TComponent, TBuilder>
    {
        private TComponent _component;
        public QueryComponentBuilderBase(TComponent component)
        {
            Guard.IsNotNull(component, "component");
            _component = component;
        }

        /// <summary>
        /// Recursive builder context.
        /// </summary>
        protected internal TComponent Component
        {
            get { return _component; }
            set { _component = value; }
        }
      
        public static implicit operator TComponent(QueryComponentBuilderBase<TComponent, TBuilder> builder)
        {
            Guard.IsNotNull(builder, "builder");

            return builder.ToComponent();
        }

        /// <summary>
        /// Returns the sql statement.
        /// </summary>
        /// <returns></returns>
        public TComponent ToComponent()
        {
            return Component;
        }

        /// <summary>
        /// Final method to build the SQL statement.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _component.ToQueryComponent();
        }
    }
}
