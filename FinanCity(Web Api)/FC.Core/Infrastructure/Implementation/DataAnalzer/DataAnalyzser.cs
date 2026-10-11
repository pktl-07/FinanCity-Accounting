namespace FC.Core.Infrastructure.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DataAnalyzser<T> : IDataAnalyzer<T>
        where T : class
    {
        private DataAnalyzerContext<T> _context = new DataAnalyzerContext<T>();
        private DataAnalyzerConfigurator<T> _config;

        public DataAnalyzser(DataAnalyzerConfigurator<T> config)
        {
            _config = config;
            ValidateKey();
            ValidateRequire();
            BuildDeleteItems();
            BuildEditItems();
            BuildInsertItems();
        }

        public DataAnalyzerContext<T> CreateContext()
        {
            return _context;
        }

        private void ValidateRequire()
        {
            if (_config.KeySelector == null)
                throw new Exception("The key should not be null");

            if (_config.OriginData == null)
                throw new Exception("The Origin Data should not be null");
        }

        private void ValidateKey()
        {
            if (_config.OriginData.Distinct(new AutoComparer(_config.KeySelector.Compile())).Count() != _config.OriginData.Count())
            {
                throw new Exception("Origin value has some duplicate keys");
            }

            var post = _config.PostData.BatchItemsWithKey(_config.KeySelector);
            if (post.Distinct(new AutoComparer(_config.KeySelector.Compile())).Count() != post.Count())
            {
                throw new Exception("Posting value has some duplicate keys");
            }
        }

        private void BuildInsertItems()
        {
            _config.PostData.BatchItemsWithoutKey(_config.KeySelector).Each(data =>
            {
                if (_config.BeforeInsert != null)
                    _config.BeforeInsert(data);

                _context.InsertItems.Add(data);
            });
        }

        private void BuildDeleteItems()
        {
            var post = _config.PostData.BatchItemsWithKey(_config.KeySelector);

            _config.OriginData.Except(post, new AutoComparer((x) => 
            _config.OriginData.BatchItemsWithSingle(x, _config.KeySelector)))
            .Each(data =>
            {
                _context.DeleteItems.Add(data);
            });
        }

        private void BuildEditItems()
        {
            var post = _config.PostData.BatchItemsWithKey(_config.KeySelector);
            post.BatchItemsNotMatch(_config.OriginData, _config.KeySelector).Each(data =>
            {
                _context.UpdateItems.Add(data);
            });
        }

        private class AutoComparer : IEqualityComparer<T>
        {
            private Func<T, object> _keySelector;
            public AutoComparer(Func<T, object> keySelector)
            {
                _keySelector = keySelector;
            }

            public bool Equals(T x, T y)
            {
                return _keySelector(x).Equals(_keySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return _keySelector(obj).GetHashCode();
            }
        }
    }
}
