using Aequus.Buffs.Buildings;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Common.Personalities
{
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
                Main.NewText(val.PriceAdjustment);
                val.PriceAdjustment = Math.Max(val.PriceAdjustment - player.Aequus().villagerHappiness, max);
                Main.NewText(val.PriceAdjustment, Microsoft.Xna.Framework.Color.Aqua);
            }
            return val;
        }

        public void Unload()
        {
        }
    }
}