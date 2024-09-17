using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FNAUtils;

public class ArrayInstancePool<T> {
    private readonly Dictionary<int, Stack<T[]>> _arrays = [];

    public T[] Get(int length) {
        Stack<T[]> stack = GetArrayStack(length);
        T[] array = stack.Count == 0 ? (new T[length]) : stack.Pop();
        return array;
    }

    public void Return(T[] array) {
        Stack<T[]> stack = GetArrayStack(array.Length);

        stack.Push(array);
    }

    private Stack<T[]> GetArrayStack(int length) {
        return CollectionsMarshal.GetValueRefOrAddDefault(_arrays, length, out _) ??= new();
    }
}
