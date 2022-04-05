using Aequus.Particles.Dusts;
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
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 4;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
            Projectile.alpha = 200;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation += 0.15f;
            Projectile.velocity.Y += 0.125f;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 3;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.frame = Main.rand.Next(Main.projFrames[Type]);
                Projectile.ai[0]++;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(222);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.Frostburn, 240);
            }
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
            float opacity = 1f - Projectile.alpha / 255f;
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f - (1f / trailLength * i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + drawOffset, frame, new Color(20, 80, 175, 10) * progress * opacity, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            foreach (var v in AequusHelpers.CircularVector(4))
            {
                Main.EntitySpriteDraw(texture, Projectile.position + drawOffset + v * 2f, frame, new Color(20, 135, 175, 10) * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.position + drawOffset, frame, Projectile.GetAlpha(lightColor) * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + drawOffset, frame, new Color(255, 255, 255, 0) * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0.45f, 1f) * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = Projectile.Center;
            var size = Projectile.Size.Length() / 2f;
            for (int i = 0; i < 80; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(60, 111, 160, 0) * Main.rand.NextFloat(0.75f, 1.2f), Main.rand.NextFloat(0.75f, 1.25f));
                Main.dust[d].position = center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(size);
                Main.dust[d].velocity = (Main.dust[d].position - center) / 2f;
            }
        }
    }
}