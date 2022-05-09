using System;

namespace Aequus.Common.Utilities
{
    public sealed class StaticManipulator<T>
    {
        private sealed class InvalidCacheException : Exception
        {
            public InvalidCacheException(string text) : base(text)
            {
            }
        }


        private readonly RefFunc<T> _ref;
        private T _myValue;
        private T _origValue;
        private bool _caching;
        public bool IsCaching => _caching;

        public StaticManipulator(RefFunc<T> reference)
        {
            _ref = reference;
            _myValue = default(T);
            _origValue = default(T);
            _caching = false;
        }

        public void StartCaching(T value)
        {
            if (_caching)
            {
                EndCaching();
                throw new InvalidCacheException("Trying to set cached data while there is already data being cached");
            }
            _myValue = value;
            _origValue = _ref.Invoke();
            _ref.Invoke() = _myValue;
            _caching = true;
        }

        public void RepairCachedStatic()
        {
            if (!_caching)
            {
                throw new InvalidCacheException("Cannot repair reference data when there is no cache");
            }
            _ref.Invoke() = _origValue;
        }

        public void DisrepairCachedStatic()
        {
            if (!_caching)
            {
                throw new InvalidCacheException("Cannot disrepair reference data when there is no cache");
            }
            _ref.Invoke() = _myValue;
        }

        public void EndCaching()
        {
            if (!_caching)
            {
                throw new InvalidCacheException("Cannot clear cached data when there is no cache");
            }
            _ref.Invoke() = _origValue;
            _myValue = default(T);
            _origValue = default(T);
            _caching = false;
        }
    }
}