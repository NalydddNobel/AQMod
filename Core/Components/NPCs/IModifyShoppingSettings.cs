using Terraria.GameContent;

namespace Aequu2.Core.Components.NPCs;

public interface IModifyShoppingSettings {
    void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper);
}