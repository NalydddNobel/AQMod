using Aequus.Buffs.Debuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class BallisticScreecherProj : ModProjectile
    {
        public override string Texture => Aequus.TextureNone;

        public override void SetStaticDefaults()
        {
            this.SetTrail(40);
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 180;
            Projectile.alpha = 200;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 100)
            {
                Projectile.velocity *= 0.995f;
            }
            if (Main.myPlayer == Projectile.owner && Main.rand.NextBool(3))
            {
                Projectile.timeLeft--;
            }
            if (Projectile.timeLeft < 50)
            {
                Projectile.alpha -= 13;
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 5;
                    if (Projectile.alpha < 0)
                    {
                        Projectile.alpha = 0;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            CrimsonHellfire.AddStack(target, 60, 1);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = Images.Bloom[0].Value;
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            var origin = frame.Size() / 2f;
            var center = Projectile.Center;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < trailLength; i++)
            {
                var p = AequusHelpers.CalcProgress(trailLength, i);
                DrawBloom(texture, Projectile.oldPos[i] + offset - Main.screenPosition, Projectile.Opacity * p * p, origin, 0.1f + 0.9f * p * p * p);
            }

            DrawBloom(texture, Projectile.position + offset - Main.screenPosition, Projectile.Opacity, origin, 1f);
            return false;
        }
        private void DrawBloom(Texture2D bloom, Vector2 where, float opacity, Vector2 origin, float scale)
        {
            scale *= 0.25f;
            Main.spriteBatch.Draw(bloom, where, null, CrimsonHellfire.BloomColor * opacity, Projectile.rotation, origin, Projectile.scale * scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloom, where, null, CrimsonHellfire.FireColor * opacity * 2f, Projectile.rotation, origin, Projectile.scale * scale, SpriteEffects.None, 0f);
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            SoundID.Item10?.PlaySound(Projectile.Center);
            var center = Projectile.Center;
            for (int i = 0; i < 20; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.velocity = (d.position - center) / 8f;
                d.noGravity = true;
                if (Main.rand.NextBool(3))
                {
                    d.velocity *= 2f;
                    d.scale *= 1.75f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
                }
            }
        }
    }
}