namespace FC.Core.Infrastructure.Implementation
{
    using System.Collections.Generic;
    using System.Linq;

    internal class LessExceptAlgorithm<T> : IExceptor<T>
    {
        private IEqualityComparer<T> _equility;
        private T[] _comparer;
        private T[] _source;

        public LessExceptAlgorithm(IEqualityComparer<T> equility, T[] source,T[] comparer)
        {
            _equility = equility;
            _source = source;
            _comparer = comparer;
        }

        public T[] GetExceptResult()
        {
            return _comparer.Except(_source, _equility).ToArray();
        }
    }
}
