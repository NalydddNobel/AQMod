using Aequus.Core;

namespace Aequus.Old.Content.Items.Weapons.Magic.Gamestar;

public class Gamestar : ModItem {
    public override void SetDefaults() {
        Item.SetWeaponValues(32, 3.5f);
        Item.DamageType = DamageClass.Magic;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.width = 20;
        Item.height = 20;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = Commons.Rare.BossOmegaStarite;
        Item.shoot = ModContent.ProjectileType<GamestarProj>();
        Item.shootSpeed = 25f;
        Item.mana = 14;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item75 with { Pitch = 1f };
        Item.value = Commons.Cost.BossOmegaStarite;
    }
}
