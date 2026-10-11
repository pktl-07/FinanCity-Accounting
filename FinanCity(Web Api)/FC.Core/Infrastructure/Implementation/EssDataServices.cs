namespace FC.Core.Infrastructure.Implementation
{
    using System.Collections.Generic;

    public class EssDataServices : IEssDataRepository
    {
        public Dictionary<string, DataConnection> DbConnection { get; set; }

        public string LoginName { get; set; }

        public int ApplicationType { get; set; }

        public string SPSK { get; set; }
    }
}
