using Aequus.Old.Content.Materials;

namespace Aequus.Content.Equipment.Accessories.EventPrevention;

[AutoloadEquip(EquipType.Face)]
public class StellarGlasses : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<EventDeactivatorPlayer>().accDisableGlimmer = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.5f);
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        color = Color.Lerp(color, Color.White, 0.5f);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BlackLens)
            .AddIngredient<StariteMaterial>(5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
