using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Common.DataSets;

internal class IDValueCollection<TID, T> : IDictionary<IDEntry<TID>, T>, IDictionary, IReadOnlyDictionary<IDEntry<TID>, T> where TID : class {
    private readonly Dictionary<IDEntry<TID>, T> _dict = [];

    public T this[IDEntry<TID> key] { get => ((IDictionary<IDEntry<TID>, T>)_dict)[key]; set => ((IDictionary<IDEntry<TID>, T>)_dict)[key] = value; }
    public object? this[object key] { get => ((IDictionary)_dict)[key]; set => ((IDictionary)_dict)[key] = value; }

    public ICollection<IDEntry<TID>> Keys => ((IDictionary<IDEntry<TID>, T>)_dict).Keys;

    public ICollection<T> Values => ((IDictionary<IDEntry<TID>, T>)_dict).Values;

    public int Count => ((ICollection<KeyValuePair<IDEntry<TID>, T>>)_dict).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<IDEntry<TID>, T>>)_dict).IsReadOnly;

    public bool IsFixedSize => ((IDictionary)_dict).IsFixedSize;

    public bool IsSynchronized => ((ICollection)_dict).IsSynchronized;

    public object SyncRoot => ((ICollection)_dict).SyncRoot;

    ICollection IDictionary.Keys => ((IDictionary)_dict).Keys;

    IEnumerable<IDEntry<TID>> IReadOnlyDictionary<IDEntry<TID>, T>.Keys => ((IReadOnlyDictionary<IDEntry<TID>, T>)_dict).Keys;

    ICollection IDictionary.Values => ((IDictionary)_dict).Values;

    IEnumerable<T> IReadOnlyDictionary<IDEntry<TID>, T>.Values => ((IReadOnlyDictionary<IDEntry<TID>, T>)_dict).Values;

    public void Add(IDEntry<TID> key, T value) {
        ((IDictionary<IDEntry<TID>, T>)_dict).Add(key, value);
    }

    public void Add(KeyValuePair<IDEntry<TID>, T> item) {
        ((ICollection<KeyValuePair<IDEntry<TID>, T>>)_dict).Add(item);
    }

    public void Add(object key, object? value) {
        ((IDictionary)_dict).Add(key, value);
    }

    public void Clear() {
        ((ICollection<KeyValuePair<IDEntry<TID>, T>>)_dict).Clear();
    }

    public bool Contains(KeyValuePair<IDEntry<TID>, T> item) {
        return ((ICollection<KeyValuePair<IDEntry<TID>, T>>)_dict).Contains(item);
    }

    public bool Contains(object key) {
        return ((IDictionary)_dict).Contains(key);
    }

    public bool ContainsKey(IDEntry<TID> key) {
        return ((IDictionary<IDEntry<TID>, T>)_dict).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<IDEntry<TID>, T>[] array, int arrayIndex) {
        ((ICollection<KeyValuePair<IDEntry<TID>, T>>)_dict).CopyTo(array, arrayIndex);
    }

    public void CopyTo(Array array, int index) {
        ((ICollection)_dict).CopyTo(array, index);
    }

    public IEnumerator<KeyValuePair<IDEntry<TID>, T>> GetEnumerator() {
        return ((IEnumerable<KeyValuePair<IDEntry<TID>, T>>)_dict).GetEnumerator();
    }

    public bool Remove(IDEntry<TID> key) {
        return ((IDictionary<IDEntry<TID>, T>)_dict).Remove(key);
    }

    public bool Remove(KeyValuePair<IDEntry<TID>, T> item) {
        return ((ICollection<KeyValuePair<IDEntry<TID>, T>>)_dict).Remove(item);
    }

    public void Remove(object key) {
        ((IDictionary)_dict).Remove(key);
    }

    public bool TryGetValue(IDEntry<TID> key, [MaybeNullWhen(false)] out T value) {
        return ((IDictionary<IDEntry<TID>, T>)_dict).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return new IDEnumerator(_dict.GetEnumerator());
    }

    IDictionaryEnumerator IDictionary.GetEnumerator() {
        return new IDEnumerator(_dict.GetEnumerator());
    }

    private readonly struct IDEnumerator(Dictionary<IDEntry<TID>, T>.Enumerator parentEnumerator) : IDictionaryEnumerator {
        private readonly Dictionary<IDEntry<TID>, T>.Enumerator _enumerator = parentEnumerator;

        public DictionaryEntry Entry => ((IDictionaryEnumerator)_enumerator).Entry;

        public object Key => ((IDictionaryEnumerator)_enumerator).Key;

        public object? Value => ((IDictionaryEnumerator)_enumerator).Value;

        public object Current => ((IEnumerator)_enumerator).Current;

        public bool MoveNext() {
            IEnumerator<KeyValuePair<IDEntry<TID>, T>> enumerator = _enumerator;
            while (enumerator.MoveNext()) {

                // End enumeration movement upon reaching a valid entry.
                if (enumerator.Current.Key.ValidEntry) {
                    return true;
                }
            }

            return false;
        }

        public void Reset() {
            ((IEnumerator)_enumerator).Reset();
        }
    }
}
