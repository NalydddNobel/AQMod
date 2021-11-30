using AQMod.Common.WorldGeneration;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class Floatstick : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            }
            projectile.velocity *= 0.98f;
            projectile.rotation += projectile.velocity.Length() * 0.0157f;

            if (Main.myPlayer != projectile.owner)
            {
                return;
            }

            Lighting.AddLight(projectile.Center, projectile.GetAlpha(Color.White).ToVector3());
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(190, 40, 220 + (int)(Math.Sin(Main.GlobalTime * 5f) * 40f));
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.drawProjAtCenter(lightColor);
            return false;
        }
    }
}