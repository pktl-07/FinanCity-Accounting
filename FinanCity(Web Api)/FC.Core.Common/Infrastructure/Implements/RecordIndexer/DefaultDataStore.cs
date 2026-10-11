namespace FC.Core.Infrastructure.Implements.RecordIndexer
{
    using FC.Core;
    using FC.Core.Data;

    public class DefaultDataStore : RecordIndexerBaseStore
    {
        public DefaultDataStore(TransType trasnType, IEntity entity,RecordIndexerEntity info) 
            : base(trasnType, entity,info)
        {
        }
    }
}
