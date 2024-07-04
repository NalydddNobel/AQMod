using System;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AutoloadStatueAttribute : Attribute, IAttributeOnModLoad {
    public void OnModLoad(Mod mod, ILoad Content) {
        if (Content is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadStatueAttribute)}  can only be applied to ModNPCs.");
        }

        mod.AddContent(new InstancedNPCStatue(modNPC));
    }
}
