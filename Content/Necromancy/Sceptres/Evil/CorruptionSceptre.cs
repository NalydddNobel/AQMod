using Aequus.Items;

namespace Aequus.Content.Necromancy.Sceptres.Evil;

[LegacyName("ZombieSceptre", "ZombieScepter")]
[AutoloadGlowMask]
public class CorruptionSceptre : ScepterBase {
    public override void SetDefaults() {
        Item.DefaultToNecromancy(30);
        Item.SetWeaponValues(10, 1f, 0);
        Item.shoot = ModContent.ProjectileType<CorruptionSceptreProj>();
        Item.shootSpeed = 9f;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 50);
        Item.mana = 20;
        Item.UseSound = SoundID.Item8;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.DemoniteBar, 6)
            .AddIngredient(ItemID.LifeCrystal)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
