using System;
using Terraria.ModLoader;

namespace Aequus.Core;

public class InstanceHook<TReturn, THook> where TReturn : class, ILoadable where THook : Delegate {
    public THook Hook { get; private set; }

    public TReturn Add(THook hook) {
        Hook = (THook)Delegate.Combine(hook, Hook);
        return ModContent.GetInstance<TReturn>();
    }
}