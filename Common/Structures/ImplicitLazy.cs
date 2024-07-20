using System;

namespace Aequus.Common.Structures;

/// <summary><see cref="Lazy{T}"/> but with an implicit operator to transform into a value.</summary>
/// <typeparam name="T"></typeparam>
public class ImplicitLazy<T> : Lazy<T> {
    public ImplicitLazy() : base() { }
    public ImplicitLazy(T value) : base(value) { }
    public ImplicitLazy(Func<T> valueFactory) : base(valueFactory) { }

    public static implicit operator T(ImplicitLazy<T> lazy) {
        return lazy.Value;
    }
}
