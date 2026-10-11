namespace FC.Core.Infrastructure.Implementation
{
    using System.Collections.Generic;
    using System.Linq;

    internal class LargeExceptAlgorithm<T> : IExceptor<T>
    {
        private IEqualityComparer<T> _equility;
        private T[] _comparer;
        public T[] _source;

        public LargeExceptAlgorithm(IEqualityComparer<T> equility, T[] source, T[] comparer)
        {
            _equility = equility;
            _source = source;
            _comparer = comparer;
        }

        public T[] GetExceptResult()
        {
            return _source.Except(_comparer, _equility).ToArray();
        }
    }
}
