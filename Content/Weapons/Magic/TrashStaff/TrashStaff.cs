using Aequus.Common.Items;
using Aequus.Content.Materials;

namespace Aequus.Content.Weapons.Magic.TrashStaff;

[LegacyName("LiquidGun")]
public class TrashStaff : ModItem {
    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<TrashStaffProj>(), 4, 14f, hasAutoReuse: true);
        Item.SetWeaponValues(17, 1f, bonusCritChance: 8);
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
        Item.mana = 10;
        Item.useAnimation *= 5;
        Item.reuseDelay = 25;
        Item.UseSound = SoundID.Item8;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<CompressedTrash>(12)
            .AddIngredient(ItemID.FallenStar, 2)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
    }
}