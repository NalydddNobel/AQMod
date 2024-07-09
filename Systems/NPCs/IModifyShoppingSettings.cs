using Terraria.GameContent;

namespace AequusRemake.Systems.NPCs;

public interface IModifyShoppingSettings {
    void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper);
}