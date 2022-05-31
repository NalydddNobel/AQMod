using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public class TrapperProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 180, 180, 0);
        }

        public override void AI()
        {
            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            Main.dust[d].velocity = Vector2.Lerp(Projectile.velocity, Main.dust[d].velocity, 0.5f);
            Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
            Main.dust[d].noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0]--;
                return;
            }
            if (Projectile.lavaWet)
            {
                var player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
                if (Projectile.Center.Y < player.Center.Y)
                {
                    Projectile.velocity.Y += 1f;
                    if (Projectile.velocity.Y > 40f)
                        Projectile.velocity.Y = 40f;
                }
                else
                {
                    Projectile.velocity.Y -= 1f;
                    if (Projectile.velocity.Y < -40f)
                        Projectile.velocity.Y = -40f;
                }
            }
            else
            {
                Projectile.velocity.Y += 1f;
                if (Projectile.velocity.Y > 20f)
                    Projectile.velocity.Y = 20f;
            }
            if (Projectile.velocity.X.Abs() < 1f)
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

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            int p = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TrapperExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Vector2 position = Projectile.Center - new Vector2(Main.projectile[p].width / 2f, Main.projectile[p].height / 2f);
            Main.projectile[p].position = position;

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            var bvelo = -Projectile.velocity * 0.4f;
            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Main.projectile[p].position, bvelo * 0.2f, 61 + Main.rand.Next(3));
            }
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, DustID.Smoke);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, DustID.Torch);
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
        }
    }

    public class TrapperExplosion : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.timeLeft = 2;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0)
                Projectile.active = false;
            Projectile.ai[0]++;
        }
    }
}
