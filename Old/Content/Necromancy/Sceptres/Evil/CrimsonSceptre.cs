using tModLoaderExtended.GlowMasks;

namespace Aequu2.Old.Content.Necromancy.Sceptres.Evil;

[AutoloadGlowMask]
public class CrimsonSceptre : ScepterBase {
    public override void SetDefaults() {
        Item.DefaultToNecromancy(30);
        Item.SetWeaponValues(10, 1f, 0);
        Item.shoot = ModContent.ProjectileType<CrimsonSceptreProj>();
        Item.shootSpeed = 9f;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 50);
        Item.mana = 20;
        Item.UseSound = SoundID.Item8;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.CrimtaneBar, 6)
            .AddIngredient(ItemID.LifeCrystal)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
