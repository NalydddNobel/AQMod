using Aequus.Content.Events.DemonSiege;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    [AutoloadGlowMask]
    [LegacyName("BallisticScreecher")]
    public class BombarderRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ItemID.CrimsonRod, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(18, 2f, 11);
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.width = 32;
            Item.height = 32;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.shoot = ModContent.ProjectileType<BombarderRodProj>();
            Item.shootSpeed = 8.5f;
            Item.mana = 6;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item88.WithVolume(0.5f).WithPitchOffset(0.8f);
            Item.value = ItemDefaults.ValueDemonSiege;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += Vector2.Normalize(velocity) * 38f;
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
        }
    }
}