using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Aequus.Core.Initialization;

namespace Aequus.Old.Content.Weapons.Demon.Magic;

[AutoloadGlowMask]
[LegacyName("BallisticScreecher")]
public class BombarderRod : ModItem {
    public override void SetStaticDefaults() {
        Item.staff[Item.type] = true;
        AltarSacrifices.Register(ItemID.CrimsonRod, Type);
    }

    public override void SetDefaults() {
        Item.SetWeaponValues(18, 2f, 11);
        Item.DamageType = DamageClass.Magic;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.width = 32;
        Item.height = 32;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.shoot = ModContent.ProjectileType<BombarderRodProj>();
        Item.shootSpeed = 8.5f;
        Item.mana = 6;
        Item.autoReuse = true;
        Item.UseSound = SoundID.Item88 with { Volume = 0.5f, Pitch = 0.8f };
        Item.rare = ItemCommons.Rarity.DemonSiegeTier1Loot;
        Item.value = ItemCommons.Price.DemonSiegeLoot;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref System.Int32 type, ref System.Int32 damage, ref System.Single knockback) {
        position += Vector2.Normalize(velocity) * 38f;
        velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
    }
}