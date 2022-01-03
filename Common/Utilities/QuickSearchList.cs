using System;

namespace AQMod.Common.Utilities
{
    /// <summary>
    /// Unfinished utility which has quick searching functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class QuickSearchList<T>
    {
        private ValueTuple<ushort, T>[] value;

        public QuickSearchList()
        {
            Clear();
        }

        private ushort gethash(T item)
        {
            if (item == null)
                return 0;
            else
                return (ushort)(item.GetHashCode() + 1);
        }

        private int getapproxindex(ushort hash)
        {
            int index = (int)(hash / (double)value[value.Length - 1].Item1 * value.Length - 1);
            while (true) // refines the index to be properly sorted... I hope
            {
                if (hash > value[index + 1].Item1)
                {
                    index++;
                }
                else if (hash < value[index].Item1)
                {
                    index--;
                }
                else
                    break;
            }
            return index;
        }

        public void Add(T item)
        {
            ushort hash = gethash(item);
            if (value.Length == 0)
            {
                Array.Resize(ref value, 1);
                value[0] = (hash, item);
            }
            else if (value.Length == 1)
            {
                if (hash == value[0].Item1)
                {
                    value[0] = (hash, item);
                }
                else if (hash < value[0].Item1)
                {
                    Array.Resize(ref value, 2);
                    value[1] = value[0];
                    value[0] = (hash, item);
                }
                else
                {
                    Array.Resize(ref value, 2);
                    value[1] = (hash, item);
                }
            }
            else
            {
                if (hash >= value[value.Length - 1].Item1)
                {
                    Array.Resize(ref value, value.Length + 1);
                    value[value.Length - 1] = (hash, item);
                }
                else if (hash <= value[0].Item1)
                {
                    Array.Resize(ref value, value.Length + 1);
                    for (int i = value.Length - 1; i > 0; i--)
                    {
                        value[i] = value[i - 1];
                    }
                    value[0] = (hash, item);
                }
                else
                {
                    int index = getapproxindex(hash);
                    if (value[index].Item1 == hash)
                    {
                        value[index] = (hash, item);
                    }
                    else
                    {
                        insert(index, hash, item);
                    }
                }
            }
        }

        private void insert(int index, ushort hash, T item)
        {
            Array.Resize(ref value, value.Length + 1);
            for (int i = value.Length - 1; i > index; i--)
            {
                value[i] = value[i - 1];
            }
            value[index] = (hash, item);
        }

        public void Clear()
        {
            value = new (ushort, T)[0];
        }

        public int Count => value.Length;

        public T[] ToArray()
        {
            T[] arr = new T[value.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value[i].Item2;
            }
            return arr;
        }

        public bool Contains(T item)
        {
            return TryFind(item.GetHashCode(), out var _);
        }
        public bool Contains(int type)
        {
            return TryFind(type, out var _);
        }
        public bool Contains(ushort type)
        {
            return TryFind(type, out var _);
        }
        public bool TryFind(int type, out T result)
        {
            return TryFind((ushort)type, out result);
        }
        public bool TryFind(ushort type, out T result)
        {
            if (type == value[value.Length - 1].Item1)
            {
                result = value[value.Length - 1].Item2;
                return true;
            }
            if (type == value[0].Item1)
            {
                result = value[value.Length - 1].Item2;
                return true;
            }
            if (type > value[value.Length - 1].Item1 || type < value[0].Item1)
            {
                result = default(T);
                return false;
            }
            int index = getapproxindex(type);
            if (value[index].Item1 == type)
            {
                result = value[index].Item2;
                return true;
            }
            result = default(T);
            return false;
        }
        public T Find(T item)
        {
            return Find(item.GetHashCode());
        }
        public T Find(int type)
        {
            return Find((ushort)type);
        }
        public T Find(ushort type)
        {
            return value[getapproxindex(type) + 1].Item2;
        }
    }
}