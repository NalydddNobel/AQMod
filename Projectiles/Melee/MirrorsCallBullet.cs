using Aequus.Effects;
using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public sealed class MirrorsCallBullet : ModProjectile
    {
        public float colorProgress;

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainbowCrystalExplosion;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 360;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            var target = Projectile.FindTargetWithinRange(400f);
            if (target != null)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * 20f, 0.5f);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                ModContent.GetInstance<ModEffects>().SetShake(2f, 2f);
                MirrorsCallExplosion.ExplosionEffects(target.Center, colorProgress, 0.75f);
                int p = Projectile.NewProjectile(Projectile.GetProjectileSource_OnHit(target, Type), target.Center,
                    Vector2.Normalize(target.Center - Main.player[Projectile.owner].Center), ModContent.ProjectileType<MirrorsCallExplosion>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner);
                Main.projectile[p].scale = 0.4f;
                Main.projectile[p].width = (int)(Main.projectile[p].width * 0.4f);
                Main.projectile[p].height = (int)(Main.projectile[p].height * 0.4f);
                Main.projectile[p].Center = target.Center;
                Main.projectile[p].Mod<MirrorsCallExplosion>().colorProgress = colorProgress + 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            MirrorsCall.DrawRainbowAura(Main.spriteBatch, texture, Projectile.Center - Main.screenPosition, null, Projectile.rotation, texture.Size() / 2f, Projectile.scale);
            MirrorsCall.DrawRainbowAura(Main.spriteBatch, texture, Projectile.Center - Main.screenPosition, null, Projectile.rotation, texture.Size() / 2f, Projectile.scale, opacity: 0.5f);
            return false;
        }
    }
}