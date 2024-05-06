using Aequus.Common.Items;
using Aequus.Items;

namespace Aequus.Content.Necromancy.Sceptres.Forbidden;

[AutoloadGlowMask]
public class Osiris : ScepterBase {
    public override void SetDefaults() {
        Item.DefaultToNecromancy(30);
        Item.SetWeaponValues(75, 1f, 0);
        Item.shoot = ModContent.ProjectileType<OsirisProj>();
        Item.shootSpeed = 12.5f;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 4);
        Item.mana = 15;
        Item.UseSound = SoundID.Item8;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.AncientBattleArmorMaterial)
            .AddIngredient(ItemID.AdamantiteBar, 10)
            .AddIngredient(ItemID.SoulofNight, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register()

            .Clone()
            .ReplaceItem(ItemID.AdamantiteBar, ItemID.TitaniumBar)
            .Register();
    }
}