using Aequus.Core;

namespace Aequus.Content.Items.Weapons.Ranged.GasCan;

[WorkInProgress]
public class GasCan : ModItem {
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.DamageType = DamageClass.Ranged;
        Item.SetWeaponValues(40, 8f);
        Item.consumable = true;
        Item.rare = Commons.Rare.BiomeOcean;
        Item.value = Item.buyPrice(silver: 1);
        Item.maxStack = Item.CommonMaxStack;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 32;
        Item.useTime = 32;

        Item.shoot = ProjectileID.Shuriken;
        Item.shootSpeed = 16f;
    }
}
