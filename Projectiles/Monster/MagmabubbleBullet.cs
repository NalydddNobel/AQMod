using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster {
    public class MagmabubbleBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 180, 180, 60);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
                d.scale *= 0.6f;
                d.fadeIn = d.scale + 0.4f;
                d.velocity *= 0.33f;
                d.noGravity = true;
            }
            if (Projectile.lavaWet)
            {
                var player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
                if (Projectile.Center.Y < player.Center.Y)
                {
                    Projectile.velocity.Y += 0.25f;
                    if (Projectile.velocity.Y > 16f)
                        Projectile.velocity.Y = 16f;
                }
                else
                {
                    Projectile.velocity.Y -= 0.25f;
                    if (Projectile.velocity.Y < -16f)
                        Projectile.velocity.Y = -16f;
                }
            }
            else
            {
                Projectile.velocity.Y += 0.25f;
                if (Projectile.velocity.Y > 8f)
                    Projectile.velocity.Y = 8f;
            }
            if (Projectile.velocity.X.Abs() < 2f)
            {
                Projectile.velocity.X *= 0.98f;
                if (Projectile.timeLeft > 60)
                    Projectile.timeLeft = 60;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)].position.Y
                > Projectile.position.Y + Projectile.height;
            return true;
        }

        private void CollisionEffects(Vector2 velocity)
        {
            Vector2 spawnPos = Projectile.position + velocity;
            for (int i = 0; i < 5; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
                d.velocity = new Vector2(0f, 2f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                d.scale *= 0.6f;
                d.fadeIn = d.scale + 0.3f;
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool doEffects = false;
            if (oldVelocity.X != Projectile.velocity.X && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                Projectile.position.X += Projectile.velocity.X * 0.9f;
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (oldVelocity.Y != Projectile.velocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                Projectile.position.Y += Projectile.velocity.Y * 0.9f;
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }
            if (doEffects)
            {
                CollisionEffects(Projectile.velocity);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item85.WithPitch(1f), Projectile.Center);
            }

            for (int i = 0; i < 18; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
                d.velocity = new Vector2(0f, 3f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                d.scale *= 0.6f;
                d.fadeIn = d.scale + 0.8f;
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}