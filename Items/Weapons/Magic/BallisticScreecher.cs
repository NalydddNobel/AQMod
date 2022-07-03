using Aequus.Biomes;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic
{
    [GlowMask]
    public class BallisticScreecher : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
            DemonSiegeInvasion.RegisterSacrifice(DemonSiegeInvasion.PHM(ItemID.CrimsonRod, Type));
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.width = 32;
            Item.height = 32;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.shoot = ModContent.ProjectileType<BallisticScreecherProj>();
            Item.shootSpeed = 5f;
            Item.mana = 3;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item109;
            Item.value = ItemDefaults.DemonSiegeValue;
            Item.knockBack = 2f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += Vector2.Normalize(velocity) * 38f;
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
        }
    }
}