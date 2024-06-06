using Aequus.Old.Content.Items.Materials.Energies;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Items.Weapons.Melee.Phasebrink;

[LegacyName("PhaseDisc")]
public class Phasebrink : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.ItemsThatAllowRepeatedRightClick[Type] = true;
    }

    public override void SetDefaults() {
        Item.width = 40;
        Item.height = 40;
        Item.SetWeaponValues(28, 3f, 0);
        Item.useTime = 19;
        Item.useAnimation = 19;
        Item.rare = ItemRarityID.LightPurple;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.value = Item.sellPrice(gold: 3);
        Item.DamageType = DamageClass.Melee;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shoot = ModContent.ProjectileType<PhasebrinkProj>();
        Item.shootSpeed = 6f;
        Item.autoReuse = true;
    }

    public override bool CanUseItem(Player player) {
        return player.ownedProjectileCounts[Item.shoot] < 7;
    }

    public override bool AltFunctionUse(Player player) {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: player.altFunctionUse == 2 ? 1 : 0);
        return false;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<Vrang.Vrang>()
            .AddIngredient(ItemID.Trimarang)
            .AddIngredient(EnergyMaterial.Atmospheric)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.LightDisc);
    }
}