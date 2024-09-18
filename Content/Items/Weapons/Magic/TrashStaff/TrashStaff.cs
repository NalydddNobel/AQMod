namespace Aequus.Content.Items.Weapons.Magic.TrashStaff;

[LegacyName("LiquidGun")]
public class TrashStaff : ModItem {
    public override void SetStaticDefaults() {
#if ELEMENTS
        Element.Water.AddItem(Type);
#endif
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<TrashStaffProj>(), 4, 14f, hasAutoReuse: true);
        Item.SetWeaponValues(17, 1f, bonusCritChance: 8);
        Item.mana = 10;
        Item.useAnimation *= 5;
        Item.reuseDelay = 25;
        Item.UseSound = SoundID.Item8;
        Item.CloneShopValues(ItemID.Trident, +1, 1.5f);
    }

    public override void AddRecipes() {
#if POLLUTED_OCEAN
        CreateRecipe()
            .AddIngredient<Materials.CompressedTrash.CompressedTrash>(25)
            .AddIngredient(ItemID.FallenStar, 10)
            .AddTile(TileID.Anvils)
            .Register();
#elif !CRAB_CREVICE_DISABLE
        CreateRecipe()
            .AddIngredient<global::Aequus.Items.Materials.PearlShards.PearlShardWhite>(3)
            .AddIngredient(ItemID.FallenStar, 10)
            .AddIngredient<global::Aequus.Items.Materials.Energies.AquaticEnergy>()
            .AddTile<global::Aequus.Tiles.CraftingStations.RecyclingMachineTile>()
            .Register();
#endif
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
    }
}