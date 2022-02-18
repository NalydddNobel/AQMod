using AQMod.Assets;
using AQMod.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.GrapplingHooks
{
    public sealed class VampireHookProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 7;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 10;
        }

        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            for (int l = 0; l < Main.maxProjectiles; l++)
            {
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == projectile.type)
                {
                    hooksOut++;
                    if (hooksOut > 1)
                        return false;
                }
            }
            return true;
        }

        public override void UseGrapple(Player player, ref int type)
        {
            int hooksOut = 0;
            int oldestHookIndex = -1;
            int oldestHookTimeLeft = 100000;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == projectile.whoAmI && Main.projectile[i].type == projectile.type)
                {
                    hooksOut++;
                    if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
                    {
                        oldestHookIndex = i;
                        oldestHookTimeLeft = Main.projectile[i].timeLeft;
                    }
                }
            }
            if (hooksOut > 1)
                Main.projectile[oldestHookIndex].Kill();
        }

        public override float GrappleRange()
        {
            return 480f;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 1;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 15f;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 15f;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];
            float playerLength = (player.Center - projectile.Center).Length();
            var chainTexture = ModContent.GetTexture(this.GetPath("_Chain"));
            float textureHeight = chainTexture.Height - 2f;
            AQGraphics.Rendering.BasicChain_UseLighting(chainTexture, projectile.Center, player.Center, Main.screenPosition);
            var texture = projectile.GetTexture();
            var drawPosition = projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(texture, drawPosition, null, new Color(120, 120, 120, 255), projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition, null, new Color(100, 100, 100, 0), projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}