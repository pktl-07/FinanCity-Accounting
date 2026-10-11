namespace FC.Core.Data.SQL.Fluent
{
    public class HavingAppendixBuilder : IAppendix
    {
        private Select _component;

        public HavingAppendixBuilder(Select component)
        {
            _component = component;
        }

        public void Append(string sql)
        {
            _component.WriteHavingAppendix(sql);
        }
    }
}
