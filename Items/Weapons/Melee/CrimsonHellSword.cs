using AQMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class CrimsonHellSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask(() => new Color(200, 200, 200, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0.9f, 1f));
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 55;
            item.useTime = 48;
            item.useAnimation = 18;
            item.autoReuse = true;
            item.rare = AQItem.RarityDemonSiege;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 1);
            item.melee = true;
            item.knockBack = 3f;
            item.shoot = ModContent.ProjectileType<BurnterizerProj>();
            item.shootSpeed = 12f;
            item.scale = 1.4f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Buffs.Debuffs.CrimsonHellfire.Inflict(target, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.DemonSiegeItemAlpha(lightColor);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int size = 16;
            var off = new Vector2(-size / 2f);
            var velo = new Vector2(speedX, speedY) * 0.1f;
            for (int i = 0; i < 25; i++)
            {
                int dustType = Main.rand.NextBool(5) ? 31 : DustID.Fire;
                var p = position + new Vector2(speedX, speedY).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) + off;
                int d = Dust.NewDust(p, size, size, dustType);
                Main.dust[d].velocity = Vector2.Lerp(Main.dust[d].velocity, velo, 0.3f);
                if (dustType != 31)
                    Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2f);
                Main.dust[d].noGravity = true;
            }
            Main.PlaySound(SoundID.DD2_BetsyFireballShot, position);
            damage = (int)(damage * 0.75f);
            return true;
        }
    }
}