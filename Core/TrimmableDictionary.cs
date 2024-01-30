using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aequus.Core;
public class TrimmableDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
    private readonly Dictionary<TKey, TValue> _dictionary = new();
    private readonly Queue<TKey> _removeQueue = new();

    public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)_dictionary)[key]; set => ((IDictionary<TKey, TValue>)_dictionary)[key] = value; }

    public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dictionary).Keys;

    public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dictionary).Values;

    public System.Int32 Count => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count;

    public System.Boolean IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

    public void Add(TKey key, TValue value) {
        ((IDictionary<TKey, TValue>)_dictionary).Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item) {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
    }

    public void Clear() {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
    }

    public System.Boolean Contains(KeyValuePair<TKey, TValue> item) {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    }

    public System.Boolean ContainsKey(TKey key) {
        return ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, System.Int32 arrayIndex) {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    }

    public System.Boolean Remove(TKey key) {
        return ((IDictionary<TKey, TValue>)_dictionary).Remove(key);
    }

    public System.Boolean Remove(KeyValuePair<TKey, TValue> item) {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);
    }

    public void RemoveEnqueue(TKey key) {
        _removeQueue.Enqueue(key);
    }

    public TKey RemoveDequeue() {
        return _removeQueue.Dequeue();
    }

    public System.Boolean TryRemoveDequeue(out TKey key) {
        return _removeQueue.TryDequeue(out key);
    }

    public void RemoveAllQueued() {
        while (_removeQueue.TryDequeue(out var key)) {
            _dictionary.Remove(key);
        }
    }

    public System.Boolean TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
        return ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    public static implicit operator Dictionary<TKey, TValue>(TrimmableDictionary<TKey, TValue> dictionary) {
        return dictionary._dictionary;
    }
}