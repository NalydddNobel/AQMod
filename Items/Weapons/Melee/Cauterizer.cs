using Aequus.Buffs.Debuffs;
using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class Cauterizer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.damage = 40;
            Item.useTime = 48;
            Item.useAnimation = 24;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = ItemDefaults.DemonSiegeValue;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 3f;
            Item.shoot = ModContent.ProjectileType<CauterizerProj>();
            Item.shootSpeed = 7f;
            Item.scale = 1.2f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(200);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            CrimsonHellfire.AddStack(target, 240, 1);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = (int)(damage * 0.75f);
            position += Vector2.Normalize(velocity) * 32f;
        }
    }
}