using System;

namespace Aequus.Common.Utilities {
    public class RefFuncCacher<T>
    {
        private readonly RefFunc<T> _ref;
        private T _myValue;
        private T _origValue;
        private bool _caching;
        public bool IsCaching => _caching;

        public RefFuncCacher(RefFunc<T> reference)
        {
            _ref = reference;
            _myValue = default(T);
            _origValue = default(T);
            _caching = false;
        }

        public void SetValue(T value)
        {
            if (_caching)
            {
                ResetValue();
                //throw new Exception("Trying to set cached data while there is already data being cached");
            }
            _myValue = value;
            _origValue = _ref.Invoke();
            _ref.Invoke() = _myValue;
            _caching = true;
        }

        public void PopCachedStatic()
        {
            if (!_caching)
            {
                throw new Exception("Cannot repair reference data when there is no cache");
            }
            _ref.Invoke() = _origValue;
        }

        public void PushCachedStatic()
        {
            if (!_caching)
            {
                throw new Exception("Cannot disrepair reference data when there is no cache");
            }
            _ref.Invoke() = _myValue;
        }

        public void ResetValue()
        {
            if (!_caching)
            {
                //throw new Exception("Cannot clear cached data when there is no cache");
                return;
            }
            _ref.Invoke() = _origValue;
            _myValue = default(T);
            _origValue = default(T);
            _caching = false;
        }
    }
}