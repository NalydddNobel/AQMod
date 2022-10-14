using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    [Obsolete("Add a ModCall type to ModCallManager.RegisterCall")]
    public interface ILegacyModCallable : ILoadable
    {
        object HandleModCall(Aequus aequus, object[] args);
    }
}