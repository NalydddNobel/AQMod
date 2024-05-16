using Aequus.Common;
using Aequus.Content.Events.DemonSiege;
using tModLoaderExtended.GlowMasks;

namespace Aequus.Old.Content.Items.Weapons.Demon.Ranged;

[AutoloadGlowMask]
public class HamaYumi : ModItem {
    public override void SetStaticDefaults() {
        AltarSacrifices.Register(ItemID.DemonBow, Type);
    }

    public override void SetDefaults() {
        Item.damage = 32;
        Item.DamageType = DamageClass.Ranged;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.width = 30;
        Item.height = 30;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = Commons.Rare.DemonSiegeTier1Loot;
        Item.shoot = ProjectileID.WoodenArrowFriendly;
        Item.shootSpeed = 12.5f;
        Item.useAmmo = AmmoID.Arrow;
        Item.UseSound = SoundID.Item5;
        Item.value = Commons.Cost.DemonSiegeLoot;
        Item.noMelee = true;
        Item.autoReuse = true;
        Item.knockBack = 6f;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(200);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        type = ModContent.ProjectileType<HamaYumiArrow>();
    }
}