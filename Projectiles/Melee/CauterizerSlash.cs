using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public class CauterizerSlash : ModProjectile
    {
        private bool _didEffects;

        public override void SetStaticDefaults()
        {
            this.SetTrail(35);
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.timeLeft = 360;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * 6;

            _didEffects = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void AI()
        {
            var r = Utils.CenteredRectangle(Projectile.Center, new Vector2(Projectile.width / 4f, Projectile.height / 4f));
            if (Collision.SolidCollision(r.TopLeft(), r.Width, r.Height))
            {
                if (Projectile.velocity.Length() > 1f)
                {
                    Projectile.velocity.Normalize();
                }
                Projectile.velocity *= 0.99f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.velocity.Length() < 1f)
            {
                Projectile.velocity *= 0.99f;
                Projectile.alpha += 5;
                Projectile.scale -= 0.01f;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.velocity *= 0.975f;
                if (Main.rand.NextBool(6))
                {
                    var d = Dust.NewDustPerfect(Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)) * Main.rand.NextFloat(-50f, 50f) * Projectile.scale, DustID.Torch, -Projectile.velocity * 0.2f, 0, default, 1.5f);
                    d.noGravity = true;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.5f);
                }
            }

            if (!_didEffects)
            {
                _didEffects = true;
                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + normal * -46f, Projectile.Center + normal * 46f, 32f * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            var bloom = TextureCache.Bloom[0].Value;

            for (int i = 0; i < trailLength; i++)
            {
                float progress = AequusHelpers.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(128, 20, 10, 30) * Projectile.Opacity * progress, Projectile.oldRot[i], origin, Projectile.scale * (0.8f + progress * 0.2f), SpriteEffects.FlipHorizontally, 0);
            }
            Main.EntitySpriteDraw(TextureCache.Bloom[0].Value, Projectile.position + offset - Main.screenPosition, null, new Color(128, 20, 10, 30) * Projectile.Opacity * 0.8f, Projectile.rotation, TextureCache.Bloom[0].Value.Size() / 2f, new Vector2(1.5f, 1f) * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}