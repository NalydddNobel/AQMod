using Aequus.Common.Items.Dedications;
using Terraria.DataStructures;

namespace Aequus.Items.Equipment.PetsUtility.LanternCat;
public class LanternCatSpawner : ModItem {
    public override void Load() {
        DedicationRegistry.Register(this, new DefaultDedication("Linnn", new Color(60, 60, 120, 255)));
    }

    public override void SetDefaults() {
        Item.DefaultToVanitypet(ModContent.ProjectileType<LanternCatPet>(), ModContent.BuffType<LanternCatBuff>());
        Item.width = 20;
        Item.height = 20;
        Item.value = Item.sellPrice(gold: 1);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        player.AddBuff(Item.buffType, 2);
        return true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BambooBlock, 12)
            .AddIngredient(ItemID.BlackThread, 12)
            .AddIngredient(ItemID.BlackInk, 1)
            .AddTile(TileID.Loom)
            .Register();
    }
}