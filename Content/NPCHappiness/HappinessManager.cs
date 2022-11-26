using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Content.NPCHappiness
{
    public class HappinessManager : ILoadable
    {
        public void Load(Mod mod)
        {
            On.Terraria.GameContent.ShopHelper.GetShoppingSettings += ShopHelper_GetShoppingSettings;
        }

        private static ShoppingSettings ShopHelper_GetShoppingSettings(On.Terraria.GameContent.ShopHelper.orig_GetShoppingSettings orig, ShopHelper self, Terraria.Player player, Terraria.NPC npc)
        {
            var val = orig(self, player, npc);
            if (npc.ModNPC is IModifyShoppingSettings modifyMood)
            {
                modifyMood.ModifyShoppingSettings(player, npc, ref val, self);
            }
            return val;
        }

        public void Unload()
        {
        }
    }
}