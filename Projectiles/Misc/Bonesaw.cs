using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class Bonesaw : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BoneGloveProj);
            Projectile.width = 50;
            Projectile.height = 50;
            AIType = ProjectileID.BoneGloveProj;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 8;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.8f, 1f));

            Projectile.oldRot[0] = Projectile.velocity.ToRotation();
            for (int i = ProjectileID.Sets.TrailCacheLength[Type] - 1; i > 0; i--)
            {
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 20;
            height = 20;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate == 0)
            {
                Projectile.Kill();
                return false;
            }
            int target = Projectile.FindTargetWithLineOfSight(800f);
            if (target != -1)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.npc[target].Center).UnNaN() * oldVelocity.Length() * 0.9f;
                return false;
            }

            if (oldVelocity.Length() > 2f)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            }

            if (Projectile.velocity.X != oldVelocity.X && oldVelocity.X.Abs() > 2f)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.9f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.9f;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
            for (int i = 0; i < 10; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            var glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Main.spriteBatch.Draw(glow, Projectile.position + off - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            var trail = ModContent.Request<Texture2D>(Texture + "_Trail").Value;
            var trailColor = new Color(255, 255, 255, 50);
            for (int i = 0; i < trailLength; i++)
            {
                float p = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(trail, Projectile.oldPos[i] + off - Main.screenPosition, frame, trailColor * p, Projectile.oldRot[i], origin, Projectile.scale * (0.8f + 0.2f * p), SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}