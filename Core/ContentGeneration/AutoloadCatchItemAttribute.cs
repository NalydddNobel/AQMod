using System;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AutoloadCatchItemAttribute : Attribute, IAttributeOnModLoad {
    public AutoloadCatchItemAttribute() { }

    public void OnModLoad(Mod mod, ILoad Content) {
        if (Content is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadCatchItemAttribute)} can only be applied to ModNPCs.");
        }

        mod.AddContent(new InstancedCaughtNPCItem(modNPC));
    }

    // Setting the catch item is handled in AutoNPCDefaults.SetDefaults
}
