using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public class Sliceflake : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.ai[0]++;
            }

            if (Projectile.alpha > 40)
            {
                if (Projectile.extraUpdates > 0)
                {
                    Projectile.extraUpdates = 0;

                }
                if (Projectile.scale > 1f)
                {
                    Projectile.scale -= 0.02f;
                    if (Projectile.scale < 1f)
                    {
                        Projectile.scale = 1f;
                    }
                }
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.02f * Projectile.direction;
            bool collding = Collision.SolidCollision(Projectile.position + new Vector2(20f, 20f), Projectile.width - 40, Projectile.height - 40);
            if (collding)
            {
                Projectile.alpha += 4;
                Projectile.velocity *= 0.9f;
            }
            Projectile.alpha += 8;
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 8;
            height = 8;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, -oldVelocity.X, 0.75f);
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -oldVelocity.Y, 0.75f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            target.AddBuff(BuffID.Frostburn2, 480);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var drawPosition = Projectile.Center;
            var drawOffset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
            float speedX = Projectile.velocity.X.Abs();
            lightColor = Projectile.GetAlpha(lightColor);
            var frame = texture.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            frame.Height -= 2;
            var origin = frame.Size() / 2f;
            float opacity = Projectile.Opacity;
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            var swishOffset = Vector2.Normalize(Projectile.velocity) * (texture.Width / 2f + 4f);
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f - (1f / trailLength * i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + drawOffset, frame, new Color(20, 80, 175, 10) * progress * opacity, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.position + drawOffset, frame, Projectile.GetAlpha(lightColor) * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            var swish = ModContent.Request<Texture2D>("Aequus/Projectiles/Melee/Swords/Swish2");
            Main.EntitySpriteDraw(swish.Value, Projectile.position + drawOffset + swishOffset, null, new Color(20, 128, 200, 100) * opacity,
                Projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(swish.Value.Width / 2f, 0f), new Vector2(Projectile.scale * 0.5f, Projectile.scale * 1.5f), SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.alpha < 200)
            {
                for (int i = 0; i < 30; i++)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, newColor: new Color(80, 155, 255, 128), Scale: 2f);
                    d.velocity *= 0.4f;
                    d.velocity += Projectile.velocity * 0.5f;
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale * 0.6f;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                }
            }
        }
    }
}