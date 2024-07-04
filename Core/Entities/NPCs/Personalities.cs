using AequusRemake.Core.Components.NPCs;
using System;
using Terraria.GameContent;

namespace AequusRemake.Core.Entities.NPCs;

public class Personalities : ILoad {
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