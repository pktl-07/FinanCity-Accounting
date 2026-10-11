namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using Ninject;
    using FC.Core;
 
    public class OverridingIterator : IEnumerator<OverridingProfileItem>
    {
        private OverridingProfileCollection _collection;
        private int _position = -1;

        public OverridingIterator(OverridingProfileCollection collection)
        {
            _collection = collection;
        }

        object IEnumerator.Current => Current;

        public OverridingProfileItem Current
        {
            get
            {
                var item = DI.Current.Get<OverridingProfileItem>();

                item.SalesOrder = _collection.Context.SalesOrder;
                item.SalesDetail = _collection.Context.SalesDetails
                    .Where(x => x.TransType == (int)TransType.InternalSalesperson
                    || x.TransType == (int)TransType.MainSalesperson)
                    .ToArray()[_position];

                return item;
            }
        }

        public bool MoveNext()
        {
            ++_position;
            return _position >= 0 &&
               _position < this._collection.Context.SalesDetails.Count();
        }

        public void Reset()
        {
            this._position = 0;
        }

        public void Dispose()
        {
        }
    }
}
