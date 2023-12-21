using Aequus.Common.Items;
using Aequus.Content.Items.Material;
using Aequus.Content.Tiles.CraftingStations.TrashCompactor;

namespace Aequus.Content.Items.Weapons.Magic.TrashStaff;

public class TrashStaff : ModItem {
    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<TrashStaffProj>(), 4, 14f, hasAutoReuse: true);
        Item.SetWeaponValues(13, 1f, bonusCritChance: 8);
        Item.rare = ItemCommons.Rarity.CrabsonLoot;
        Item.value = ItemCommons.Price.CrabsonLoot;
        Item.mana = 10;
        Item.useAnimation *= 5;
        Item.reuseDelay = 15;
        Item.UseSound = SoundID.Item8;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<CompressedTrash>(12)
            .AddIngredient(ItemID.FallenStar, 2)
            .AddTile(TileID.Anvils)
            .Register();
    }
}