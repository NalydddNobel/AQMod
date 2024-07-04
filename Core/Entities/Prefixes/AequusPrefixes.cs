using AequusRemake.Core.Entities.Items.Components;
using System.Collections.Generic;
using Terraria.Utilities;

namespace AequusRemake.Core.Entities.Prefixes;

public class AequusRemakePrefixes : GlobalItem {
    internal static List<CooldownPrefix> RegisteredCooldownPrefixes { get; private set; } = new();

    public override void Unload() {
        RegisteredCooldownPrefixes?.Clear();
    }

    public override int ChoosePrefix(Item item, UnifiedRandom rand) {
        if (item.ModItem is ICooldownItem && rand.NextBool(4)) {
            return rand.Next(RegisteredCooldownPrefixes).Type;
        }

        return -1;
    }
}