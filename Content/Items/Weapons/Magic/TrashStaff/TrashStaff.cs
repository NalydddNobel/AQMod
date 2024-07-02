using Aequus.Core;
using Aequus.Content.Elements;
using Aequus.Content.Items.Materials;

namespace Aequus.Content.Items.Weapons.Magic.TrashStaff;

[LegacyName("LiquidGun")]
public class TrashStaff : ModItem {
    public override void SetStaticDefaults() {
        Element.Water.AddItem(Type);
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<TrashStaffProj>(), 4, 14f, hasAutoReuse: true);
        Item.SetWeaponValues(17, 1f, bonusCritChance: 8);
        Item.rare = Commons.Rare.BiomeOcean;
        Item.value = Commons.Cost.BiomeOcean;
        Item.mana = 10;
        Item.useAnimation *= 5;
        Item.reuseDelay = 25;
        Item.UseSound = SoundID.Item8;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<CompressedTrash>(25)
            .AddIngredient(ItemID.FallenStar, 10)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
    }
}