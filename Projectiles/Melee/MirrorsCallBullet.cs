using Aequus.Effects;
using Aequus.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public sealed class MirrorsCallBullet : ModProjectile
    {
        public float colorProgress;

        public override string Texture => "Aequus/Assets/Bullet";
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
                EffectsSystem.Shake.Set(4f);
                MirrorsCallExplosion.ExplosionEffects(target.Center, colorProgress, 0.75f);
                int p = Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center,
                    Vector2.Normalize(target.Center - Main.player[Projectile.owner].Center), ModContent.ProjectileType<MirrorsCallExplosion>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack, Projectile.owner);
                Main.projectile[p].scale = 0.4f;
                Main.projectile[p].width = (int)(Main.projectile[p].width * 0.4f);
                Main.projectile[p].height = (int)(Main.projectile[p].height * 0.4f);
                Main.projectile[p].Center = target.Center;
                Main.projectile[p].ModProjectile<MirrorsCallExplosion>().colorProgress = colorProgress + 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var bloom = Aequus.MyTex("Assets/Bloom");
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            MirrorsCall.DrawRainbowAura(Main.spriteBatch, bloom, drawCoordinates, null, 0f, bloom.Size() / 2f, Projectile.scale * 0.55f, opacity: 0.5f, drawWhite: false, rainbowScaleMultiplier: 0.6f, rainbowOffsetScaleMultiplier: 8f);
            MirrorsCall.DrawRainbowAura(Main.spriteBatch, bloom, drawCoordinates, null, 0f, bloom.Size() / 2f, Projectile.scale * 0.7f, opacity: 0.25f, drawWhite: false, rainbowScaleMultiplier: 0.75f, rainbowOffsetScaleMultiplier: 8f);
            MirrorsCall.DrawRainbowAura(Main.spriteBatch, texture, drawCoordinates, null, Projectile.rotation, texture.Size() / 2f, Projectile.scale);
            MirrorsCall.DrawRainbowAura(Main.spriteBatch, texture, drawCoordinates, null, Projectile.rotation, texture.Size() / 2f, Projectile.scale, opacity: 0.5f, drawWhite: false);
            return false;
        }
    }
}