using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Aequus.Core.Initialization;

namespace Aequus.Old.Content.Weapons.Ranged.Demon;

[AutoloadGlowMask]
public class Deltoid : ModItem {
    public override void SetStaticDefaults() {
        AltarSacrifices.Register(ItemID.TendonBow, Type);
    }

    public override void SetDefaults() {
        Item.damage = 13;
        Item.DamageType = DamageClass.Ranged;
        Item.useTime = 8;
        Item.useAnimation = Item.useTime * 3;
        Item.reuseDelay = 20;
        Item.width = 20;
        Item.height = 30;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = ItemCommons.Rarity.DemonSiegeTier1Loot;
        Item.shoot = ProjectileID.WoodenArrowFriendly;
        Item.shootSpeed = 12f;
        Item.useAmmo = AmmoID.Arrow;
        Item.UseSound = SoundID.Item5;
        Item.value = ItemCommons.Price.DemonSiegeLoot;
        Item.noMelee = true;
        Item.autoReuse = true;
        Item.knockBack = 3f;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(200);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        position += new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
        velocity = Vector2.Normalize(Main.MouseWorld - position) * velocity.Length();

        if (type == ProjectileID.WoodenArrowFriendly) {
            type = ModContent.ProjectileType<DeltoidArrow>();
        }
    }
}