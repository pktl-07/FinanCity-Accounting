namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using FC.Domain.Sales.Entity;
  
    public class OverridingProfileCollection : IEnumerable<OverridingProfileItem>
    {
        private OverridingProfileContext _context;
        public OverridingProfileContext Context
        {
            get => _context;
        }

        public IEnumerator<OverridingProfileItem> GetEnumerator()
        {
            return new OverridingIterator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void AddContext(PropertyTransactionEntity salesOrder,
            IEnumerable<PropertyTransactionDetailEntity> salesDetail)
        {
            _context = new OverridingProfileContext
            {
                SalesOrder = salesOrder,
                SalesDetails = salesDetail.ToList(),
            };
        }
    }
}
