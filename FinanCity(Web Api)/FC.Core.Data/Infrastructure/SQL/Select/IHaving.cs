namespace FC.Core.Data.SQL
{
    using System;
  
    public interface IHaving : IGroupBy  
    {
        IGroupBy Having(Action<IAppendix> action);
    }
}
