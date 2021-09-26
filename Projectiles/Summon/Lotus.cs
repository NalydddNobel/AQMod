using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public class Lotus : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            Main.projFrames[projectile.type] = 3;
        }

        private const int HeightCache = 60;

        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = HeightCache;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.sentry = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft *= 2;
            projectile.aiStyle = -1;
            projectile.gfxOffY = 10f;
        }

        public static Vector2 FindSpawn(Vector2 spawn)
        {
            int x = (int)spawn.X / 16;
            int y = (int)spawn.Y / 16;
            for (int j = 0; j < 100; j++)
            {
                if (y + j + 1 >= Main.maxTilesY)
                {
                    break;
                }
                if (Framing.GetTileSafely(x, y + j).active() && Main.tileSolid[Main.tile[x, y + j].type])
                {
                    return new Vector2(x * 16f + 8f, (y + j) * 16f - HeightCache / 2f);
                }
            }
            return new Vector2(x * 16f + 8f, y * 16f - HeightCache / 2f);
        }

        private const float ORB_RESPAWN_TIME = 25f;

        public override void AI()
        {
            if (projectile.ai[1] < 0f)
            {
                projectile.ai[1]++;
                if (projectile.ai[1] % ORB_RESPAWN_TIME == 0f)
                {
                    projectile.ai[0]++;
                }
            }
            else if (projectile.ai[0] > 0f)
            {
                if (projectile.ai[1] > 150)
                {
                    if (projectile.frameCounter % 2 == 0)
                    {
                        projectile.frameCounter++;
                    }
                }
                projectile.frameCounter++;
                if (projectile.frameCounter > 20)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                    if (projectile.frame > 1)
                    {
                        projectile.frame = 0;
                    }
                }
                projectile.ai[1]++;
            }
            else
            {
                projectile.ai[0] = Main.rand.Next(8) + 3;
                projectile.netUpdate = true;
            }
            if (projectile.ai[1] > 360f)
            {
                int type = ModContent.ProjectileType<LotusBall>();
                float rotOffset = MathHelper.PiOver2 / (projectile.ai[0] - 1f);
                var bottom = new Vector2(projectile.position.X + projectile.width / 2f, projectile.position.Y + projectile.height);
                for (int i = 0; i < projectile.ai[0]; i++)
                {
                    var pos = projectile.Center + new Vector2(0, 20f) + new Vector2(0f, -16f).RotatedBy(rotOffset * i - MathHelper.PiOver4);
                    Projectile.NewProjectile(pos, Vector2.Normalize(pos - bottom) * 3f, type, projectile.damage, projectile.knockBack, projectile.owner, 0f, projectile.position.Y);
                }
                projectile.ai[0] = -0f;
                projectile.ai[1] = (Main.rand.Next(8) + 3) * -ORB_RESPAWN_TIME;
                projectile.frame = 2;
                projectile.frameCounter = 0;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture;
            Rectangle frame;
            if (projectile.ai[0] > 0)
            {
                texture = Main.projectileTexture[ModContent.ProjectileType<LotusBall>()];
                frame = new Rectangle(0, 0, texture.Width, texture.Height);
                float rotOffset = 2f / (projectile.ai[0] - 1f);
                for (int i = 0; i < projectile.ai[0]; i++)
                {
                    LotusBall.Draw(projectile.Center + new Vector2(0f, projectile.gfxOffY) + new Vector2(0, 10f) + new Vector2(0f, -16f).RotatedBy(rotOffset * i - 1f), projectile.rotation, projectile.scale, i + projectile.whoAmI, 8f);
                }
            }
            texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            frame = new Rectangle(0, frameHeight * projectile.frame + 2, texture.Width, frameHeight - 2);
            spriteBatch.Draw(texture, projectile.Center + new Vector2(0f, projectile.gfxOffY) - Main.screenPosition, frame, lightColor, projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(projectile.whoAmI);
        }
    }
}