using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.UI.SlotDecals;

public class SlotDecalsLoader : ModSystem {
    internal static readonly List<SlotDecal> _registeredDecals = new();

    public override bool IsLoadingEnabled(Mod mod) {
        return Main.netMode != NetmodeID.Server;
    }

    public static void Register(SlotDecal decal) {
        _registeredDecals.Add(decal);
    }

    public override void Unload() {
        _registeredDecals.Clear();
    }
}