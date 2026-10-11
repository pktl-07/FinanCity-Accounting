namespace FC.Core
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core.Infrastructure;
    using FC.Core.Infrastructure.Implementation;

    public class ExceptorStrategy<T>
    {
        private IEqualityComparer<T> _equility;
        private IExceptor<T> _exceptor;
        private bool _copyMatchingValue;
        private T[] _source;

        public ExceptorStrategy(params T[] source) : this(null, source) { }

        public ExceptorStrategy(bool copyMatch, params T[] source) : this(copyMatch, null, source) { }

        public ExceptorStrategy(IEqualityComparer<T> equility, params T[] source) : this(false, equility, source) { }

        public ExceptorStrategy(bool copyMatchValue, IEqualityComparer<T> equility, params T[] source)
        {
            Guard.IsNotNull(source, "source");

            _copyMatchingValue = copyMatchValue;

            _equility = equility;

            _source = source;
        }

        public ICollection<T> GetCompareResult(params T[] comparer)
        {
            Guard.IsNotNull(comparer, "comparer");

            SwitchStrategy(comparer);

            var results = _exceptor.GetExceptResult();

            if (_copyMatchingValue)
                _source.ToList().AddRange(results);

            return results;
        }

        private void SwitchStrategy(params T[] comparer)
        {
            if (_source.Count() > comparer.Count())
                _exceptor = new LargeExceptAlgorithm<T>(_equility, _source, comparer);
            else
                _exceptor = new LessExceptAlgorithm<T>(_equility, _source, comparer);
        }
    }
}
