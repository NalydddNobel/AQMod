using AQMod.Assets;
using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Fishing
{
    public class NimrodElectric : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.BobberWooden);
            drawOriginOffsetY = -8;
        }

        public override void PostAI()
        {
            projectile.rotation = 0f;
            if ((int)projectile.ai[0] < 1)
            {
                bool shouldKill = true;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<Nimrod>())
                    {
                        shouldKill = false;
                    }
                }
                if (shouldKill)
                {
                    projectile.ai[0] = 1f;
                    projectile.Center = Main.player[projectile.owner].Center;
                }
                if (Main.raining)
                {
                    if (projectile.ai[1] < -10)
                    {
                        projectile.ai[1] += 5;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.Kill();
            return true;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            var player = Main.player[projectile.owner];
            if (!projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
                return false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].bobber && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<Nimrod>())
                {
                    AQGraphics.Rendering.FishingLine_NoLighting_UseCustomOrigin(new Color(0, 172, 255, 200), player, projectile.position, projectile.width / 2, projectile.height, projectile.velocity, projectile.localAI[0], Main.projectile[i].Center + new Vector2(Main.projectile[i].width / -2f, 0f));
                    break;
                }
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, lightColor, projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}