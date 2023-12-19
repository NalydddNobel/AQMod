using Aequus.Common.NPCs.Components;
using System;
using Terraria.GameContent;

namespace Aequus.Common.NPCs;

public class Personalities : ILoadable {
    public void Load(Mod mod) {
        On_ShopHelper.GetShoppingSettings += ShopHelper_GetShoppingSettings;
    }

    private static ShoppingSettings ShopHelper_GetShoppingSettings(On_ShopHelper.orig_GetShoppingSettings orig, ShopHelper self, Player player, NPC npc) {
        var val = orig(self, player, npc);
        if (npc.ModNPC is IModifyShoppingSettings modifyMood) {
            modifyMood.ModifyShoppingSettings(player, npc, ref val, self);
        }
        var max = Math.Min(0.85, val.PriceAdjustment);
        return val;
    }

    public void Unload() {
    }
}