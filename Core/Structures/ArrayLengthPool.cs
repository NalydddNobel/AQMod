using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aequus.Core.Structures;

public class ArrayLengthPool<T> {
    private readonly Dictionary<int, T[]> _arrays = [];

    public T[] Get(int length) {
        return CollectionsMarshal.GetValueRefOrAddDefault(_arrays, length, out _) ??= new T[length];
    }
}
