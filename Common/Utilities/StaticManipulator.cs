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

        public delegate ref T GetReference();

        private readonly GetReference _ref;
        private T _myValue;
        private T _origValue;
        private bool _caching;
        public bool Caching => _caching;

        public StaticManipulator(GetReference reference)
        {
            _ref = reference;
            _myValue = default(T);
            _origValue = default(T);
            _caching = false;
        }

        public void TemporarilySet(T value)
        {
            if (_caching)
            {
                Clear();
                throw new InvalidCacheException("Trying to set cached data while there is already data being cached");
            }
            _myValue = value;
            _origValue = _ref.Invoke();
            _ref.Invoke() = _myValue;
            _caching = true;
        }

        public void Repair()
        {
            if (!_caching)
            {
                throw new InvalidCacheException("Cannot repair reference data when there is no cache");
            }
            _ref.Invoke() = _origValue;
        }

        public void Disrepair()
        {
            if (!_caching)
            {
                throw new InvalidCacheException("Cannot disrepair reference data when there is no cache");
            }
            _ref.Invoke() = _myValue;
        }

        public void Clear()
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