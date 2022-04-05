using Aequus.Items.Weapons.Melee;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public sealed class MirrorsCallExplosion : ModProjectile
    {
        public float colorProgress;

        public override string Texture => Aequus.TextureNone;
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.timeLeft = 6;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = Projectile.timeLeft * 2;
            Projectile.penetrate = -1;
        }

        public static void ExplosionEffects(Vector2 location, float colorProgress, float scale)
        {
            int amt = (int)(90 * scale);
            var clrs = MirrorsCall.EightWayRainbow;
            for (int i = 0; i < amt; i++)
            {
                var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(40 * scale);
                var d = Dust.NewDustPerfect(location + v, ModContent.DustType<MonoDust>(), v / 2.5f, 0, AequusHelpers.LerpBetween(clrs, colorProgress + Main.rand.NextFloat(-0.2f, 0.2f)).UseA(0) * Main.rand.NextFloat(0.6f, 1.1f) * scale, Main.rand.NextFloat(0.8f, 1.8f));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner && Projectile.scale >= 0.4f)
            {
                int p = Projectile.NewProjectile(Projectile.GetProjectileSource_OnHit(target, Type), target.Center, 
                    Vector2.Normalize(Projectile.Center - Main.player[Projectile.owner].Center), Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[p].scale = Projectile.scale - 0.2f;
                Main.projectile[p].Mod<MirrorsCallExplosion>().colorProgress = colorProgress + 1f;
            }
        }
    }
}