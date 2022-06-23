using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.DustDevil
{
    public class DustDevilTornadoBullet : ModProjectile
    {
        public override string Texture => Aequus.VanillaTexture + "Projectile_" + ProjectileID.SandnadoHostile;

        public override void SetStaticDefaults()
        {
            this.SetTrail(40);
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0f)
            {
                Projectile.velocity *= 0.98f;
                Projectile.ai[1]--;
                if (Projectile.ai[1] < 0f)
                {
                    Projectile.ai[1] = 0f;
                }
            }
            else if (Projectile.ai[0] > 0f)
            {
                byte plr = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[plr].Center - Projectile.Center) * 4f, 0.0125f);
                Projectile.ai[0] -= 0.75f;
                if (Projectile.ai[0] < 0f)
                {
                    Projectile.ai[0] = 0f;
                }
            }

            Projectile.rotation += 0.1314f / 3f;

            if (Projectile.timeLeft < 255)
            {
                Projectile.alpha++;
            }

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            if (Main.rand.NextBool(6))
            {
                var n = Main.rand.NextVector2Unit();
                var d = Dust.NewDustPerfect(Projectile.Center + n * 32f * Projectile.scale * Main.rand.NextFloat(0.5f, 1f), ModContent.DustType<MonoDust>(),
                    n.RotatedBy(MathHelper.PiOver2) * 3f, newColor: Color.White.UseA(128) * 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);
            var drawColor = Color.White * 0.4f * Projectile.Opacity;
            drawColor.A = 0;

            for (int i = 0; i < trailLength; i++)
            {
                var p = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, drawColor * p,
                    Projectile.oldRot[i], origin, Projectile.scale * (0.6f + 0.4f * p), SpriteEffects.None, 0f);
            }
            trailLength /= 2;
            for (int i = 0; i < trailLength; i++)
            {
                var p = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, drawColor * p,
                    Projectile.oldRot[i], origin, Projectile.scale * (0.6f + 0.4f * p) * 1.1f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}