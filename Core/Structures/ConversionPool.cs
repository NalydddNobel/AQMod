using System;
using System.Collections.Generic;

namespace AequusRemake.Core.Structures;

public class ConversionPool<TFrom, TTo>(Func<TFrom, TTo> converter) {
    private readonly Dictionary<TFrom, TTo> _conversionCache = [];
    private readonly Func<TFrom, TTo> _converter = converter;

    public TTo Get(TFrom from) {
        return _conversionCache.TryGetValue(from, out var result) ? result : Put(from);
    }

    public bool ContainsCache(TFrom from) {
        return _conversionCache.ContainsKey(from);
    }

    private TTo Put(TFrom from) {
        TTo value = _converter(from);
        _conversionCache[from] = value;
        return value;
    }
}

public class ConversionPool<T>(Func<T, T> converter) : ConversionPool<T, T>(converter) { }