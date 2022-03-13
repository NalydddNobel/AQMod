using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class CrabsonPearlShard : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 80;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.2f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, AQUtils.Wave(Main.GlobalTime * 5f, 0.6f, 1f));
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.timeLeft < 60)
            {
                projectile.tileCollide = true;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = true;
            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 30);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            var center = projectile.Center;
            Main.PlaySound(SoundID.Item50.SoundId, (int)center.X, (int)center.Y, SoundID.Item50.Style, 0.5f, -0.1f);
            float dustRadius = projectile.Size.Length() * 0.6f;
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(dustRadius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (Main.dust[d].position - center) * 3f;
            }
            for (int i = 0; i < 2; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(120, 160, 255, 10), Main.rand.NextFloat(1.5f, 2f));
                Main.dust[d].position = center + new Vector2(Main.rand.NextFloat(dustRadius), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].velocity = (Main.dust[d].position - center) * 4.5f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f) - Main.screenPosition;
            var origin = texture.Size() / 2f;
            var drawColor = projectile.GetAlpha(lightColor);
            spriteBatch.Draw(texture, projectile.position + offset, null, drawColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}