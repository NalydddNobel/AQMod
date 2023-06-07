using Aequus.Content.Building.Bonuses;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Common.Personalities {
    public class HappinessHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            Terraria.GameContent.On_ShopHelper.GetShoppingSettings += ShopHelper_GetShoppingSettings;
        }

        private static ShoppingSettings ShopHelper_GetShoppingSettings(Terraria.GameContent.On_ShopHelper.orig_GetShoppingSettings orig, ShopHelper self, Player player, NPC npc)
        {
            var val = orig(self, player, npc);
            if (npc.ModNPC is IModifyShoppingSettings modifyMood)
            {
                modifyMood.ModifyShoppingSettings(player, npc, ref val, self);
            }
            float max = 0.85f;
            if (player.HasBuff<FountainBountyBuff>() && val.PriceAdjustment > max)
            {
                val.PriceAdjustment = Math.Max(val.PriceAdjustment + player.Aequus().shopPriceMultiplier, max);
            }
            return val;
        }

        public void Unload()
        {
        }
    }
}