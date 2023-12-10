using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Aequus.Core.Reflection;

public class MethodList<T> : IEnumerable<T>, IMethodList where T : ILoadable {
    private readonly MethodInfo _method;

    public T[] Values { get; private set; }

    public MethodList(MethodInfo method) {
        _method = method;
    }

    public void Init() {
        Values = Aequus.Instance.GetContent<T>().WhereMethodIsOverridden(_method).ToArray();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        foreach (T m in Values) {
            yield return m;
        }
    }

    public IEnumerator<T> GetEnumerator() {
        foreach (T m in Values) {
            yield return m;
        }
    }

    public static MethodList<T> Create<F>(Expression<Func<T, F>> func) where F : Delegate {
        var methodList = new MethodList<T>(func.ToMethodInfo());
        MethodListWatcher._methodListsToLoad.Add(methodList);
        return methodList;
    }
}