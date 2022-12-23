using Terraria;
using Terraria.GameContent;

namespace Aequus.Content.Personalities
{
    public interface IModifyShoppingSettings
    {
        void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper);
    }
}