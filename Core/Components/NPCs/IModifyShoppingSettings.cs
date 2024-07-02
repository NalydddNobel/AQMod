using Terraria.GameContent;

namespace Aequus.Core.Components.NPCs;

public interface IModifyShoppingSettings {
    void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper);
}