using Aequus.Core.Initialization;
using System;

namespace Aequus.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AutoloadStatueAttribute : AutoloadXAttribute {
    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadStatueAttribute)}  can only be applied to ModNPCs.");
        }

        modNPC.Mod.AddContent(new InstancedNPCStatue(modNPC));
    }
}
