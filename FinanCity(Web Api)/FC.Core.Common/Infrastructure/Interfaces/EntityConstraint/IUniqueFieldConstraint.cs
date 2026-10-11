namespace FC.Core
{
    using FC.Core.Data;
  
    public interface IUniqueFieldConstraint<T> where T : IEntity
    {
        void UniqueField(IUniqueFieldAnalyzer<T> action);
    }
}
