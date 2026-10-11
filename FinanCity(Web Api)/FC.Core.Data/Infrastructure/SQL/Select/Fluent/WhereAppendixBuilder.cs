namespace FC.Core.Data.SQL.Fluent
{
    using FC.Core.Data.Infrastructure;

    public class WhereAppendixBuilder : IAppendix
    {
        private QueryComponentBase _component;

        public WhereAppendixBuilder(QueryComponentBase component)
        {
            _component = component;
        }

        public void Append(string sql)
        {
            _component.WrieAppendixWhere(sql);
        }
    }
}
