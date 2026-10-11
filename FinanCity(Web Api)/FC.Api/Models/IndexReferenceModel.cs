namespace FC.Api.Models
{
    using System.Collections.Generic;
    using FC.Core;
    using FC.Domain.Sales.Entity;

    public class IndexReferenceModel
    {
        public IDIndexingEntity Indexer { get; set; }
        public List<BaseDate> BaseDateCollection { get; set; }
        public string TransTypeDescription
        {
            get
            {
                return ((TransType)Indexer.TransType).ToDescription();
            }
        }
    }

    public class BaseDate
    {
        public bool IsCheck { get; set; }
        public string PhysicalFieldName { get; set; }
        public string LogicalFieldName { get; set; }
    }
}