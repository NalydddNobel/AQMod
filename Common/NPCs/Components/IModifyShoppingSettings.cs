using Terraria.GameContent;

namespace Aequus.Common.NPCs.Components;

public interface IModifyShoppingSettings {
    void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper);
}