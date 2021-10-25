using AQMod.Common.WorldGeneration;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class FertilePowder : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.hide = true;
        }

        public override void AI()
        {
            projectile.velocity *= 0.95f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] == 180f)
            {
                projectile.Kill();
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 3, projectile.velocity.X, projectile.velocity.Y, 50);
                    Main.dust[d].noGravity = true;
                }
            }
            int minX = (int)(projectile.position.X / 16f) - 1;
            int maxX = (int)((projectile.position.X + projectile.width) / 16f) + 2;
            int minY = (int)(projectile.position.Y / 16f) - 1;
            int maxY = (int)((projectile.position.Y + projectile.height) / 16f) + 2;
            if (minX < 0)
            {
                minX = 0;
            }
            if (maxX > Main.maxTilesX)
            {
                maxX = Main.maxTilesX;
            }
            if (minY < 0)
            {
                minY = 0;
            }
            if (maxY > Main.maxTilesY)
            {
                maxY = Main.maxTilesY;
            }
            if (Main.myPlayer != projectile.owner)
            {
                return;
            }

            for (int k = 0; k < 10; k++)
            {
                for (int i = minX; i < maxX; i++)
                {
                    for (int j = minY; j < maxY; j++)
                    {
                        Vector2 pos = new Vector2(i * 16, j * 16);
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                        }
                        if (!(projectile.position.X + projectile.width > pos.X) || !(projectile.position.X < pos.X + 16f) || !(projectile.position.Y + projectile.height > pos.Y) || !(projectile.position.Y < pos.Y + 16f) || !Main.tile[i, j].active())
                        {
                            continue;
                        }
                        if (AQWorldGen.ActiveAndFullySolid(i, j))
                        {
                            int d = Dust.NewDust(pos, 16, 16, ModContent.DustType<MonoDust>(), 0f, 0f, 254, new Color(220 + (int)(Main.DiscoR / 255f * 30f), 220 + (int)(Main.DiscoG / 255f * 30f), 220 + (int)(Main.DiscoB / 255f * 30f), 255), 1.65f);
                            Main.dust[d].velocity *= 0f;
                        }
                        AQWorldGen.RandomUpdateTile_Spreading(i, j, maxValue: -1);
                        AQWorldGen.RandomUpdateTile_Misc(i, j, maxValue: -1);
                    }
                }
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}