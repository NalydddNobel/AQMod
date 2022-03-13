using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class CrabsonPearl : ModProjectile
    {
        private float _light;

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.scale = 0.6f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, AQUtils.Wave(Main.GlobalTime * 5f, 0.6f, 1f));
        }

        public override void AI()
        {
            projectile.velocity *= 0.96f;
            int explodeTime = 16;
            if (projectile.timeLeft < explodeTime)
            {
                projectile.tileCollide = true;
                _light += 1f / explodeTime;
                var center = projectile.Center;
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(projectile.Size.Length() * 1.2f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (center - Main.dust[d].position) / 8f;
            }
            Lighting.AddLight(projectile.Center, new Vector3(0.1f, 0.2f, 0.6f));
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)].position.Y
                > projectile.position.Y + projectile.height;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 2f)
            {
                projectile.velocity.Y = -oldVelocity.Y * 1.1f;
                if (projectile.wet)
                {
                    if (projectile.velocity.Y < -16f)
                        projectile.velocity.Y = -16f;
                }
                else
                {
                    if (projectile.velocity.Y < -8f)
                        projectile.velocity.Y = -8f;
                    Main.PlaySound(SoundID.Item10, projectile.position);
                }
            }
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X * 0.9f;
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            Main.PlaySound(SoundID.Shatter, center);
            float dustRadius = projectile.Size.Length() * 0.6f;
            for (int i = 0; i < 35; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(dustRadius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (Main.dust[d].position - center) / 2f;
            }
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10), Main.rand.NextFloat(1.5f, 2f));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(dustRadius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (Main.dust[d].position - center) / 3f;
            }
            if ((int)projectile.ai[1] == 1)
            {
                float add = Main.expertMode ? MathHelper.PiOver4 : MathHelper.PiOver2;
                for (float r = 0; r <= MathHelper.TwoPi; r += add + 0.001f)
                {
                    var normal = (r + Main.rand.NextFloat(-0.01f, 0.01f)).ToRotationVector2();
                    int p = Projectile.NewProjectile(center + normal * 20f, normal * 10f, ModContent.ProjectileType<CrabsonPearlShard>(), projectile.damage, projectile.knockBack, projectile.owner);
                    Main.projectile[p].timeLeft += Main.rand.Next(-10, 10);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f) - Main.screenPosition;
            var origin = texture.Size() / 2f;
            var drawColor = projectile.GetAlpha(lightColor);
            spriteBatch.Draw(texture, projectile.position + offset, null, drawColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            if (_light > 0f)
            {
                spriteBatch.Draw(ModContent.GetTexture(this.GetPath("_White")), projectile.position + offset, null, Color.White * _light, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}