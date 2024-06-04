using System;
using System.Collections.Generic;

namespace Aequus.Core;

public class ConversionPool<TFrom, TTo> {
    private readonly Dictionary<TFrom, TTo> _conversionCache;
    private readonly Func<TFrom, TTo> _converter;

    public ConversionPool(Func<TFrom, TTo> converter) {
        _converter = converter;
        _conversionCache = [];
    }

    public TTo Get(TFrom from) {
        if (_conversionCache.TryGetValue(from, out var result)) return result;
        return Put(from);
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