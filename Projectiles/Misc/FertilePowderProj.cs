using Aequus.Common;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class FertilePowderProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.ai[0]++;
            if (Projectile.velocity.Length() < 1f)
            {
                Projectile.ai[0] = 180f;
            }
            if ((int)Projectile.ai[0] == 180)
            {
                Projectile.Kill();
            }
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 3, Projectile.velocity.X, Projectile.velocity.Y, 50);
                    Main.dust[d].noGravity = true;
                }
            }
            int minX = (int)(Projectile.position.X / 16f) - 1;
            int maxX = (int)((Projectile.position.X + Projectile.width) / 16f) + 2;
            int minY = (int)(Projectile.position.Y / 16f) - 1;
            int maxY = (int)((Projectile.position.Y + Projectile.height) / 16f) + 2;
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
            if (Projectile.velocity.Length() > 5f)
            {
                var loc = new Vector2(Projectile.position.X + Main.rand.NextFloat(Projectile.width), Projectile.position.Y + Main.rand.NextFloat(Projectile.height));
                if (!Collision.SolidCollision(loc, 2, 2))
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FoodPiece, 0f, 0f, 254, new Color(60, 180, 20, 255), Main.rand.NextFloat(0.8f, 1.5f));
                    d.velocity = Projectile.velocity * 0.3f;
                }
            }
            if (Main.myPlayer != Projectile.owner)
            {
                return;
            }

            var map = new TileMapCache(new Rectangle(minX - 15, minY - 15, maxX - minX + 30, maxY - minY + 30).Fluffize(padding: 10));
            for (int k = 0; k < 2; k++)
            {
                for (int i = minX; i < maxX; i++)
                {
                    for (int j = minY; j < maxY; j++)
                    {
                        Vector2 pos = new Vector2(i * 16, j * 16);
                        if (!(Projectile.position.X + Projectile.width > pos.X) || !(Projectile.position.X < pos.X + 16f) || !(Projectile.position.Y + Projectile.height > pos.Y) || !(Projectile.position.Y < pos.Y + 16f) || !Main.tile[i, j].HasTile)
                        {
                            continue;
                        }
                        var cache = new TileDataCache(Main.tile[i, j]);
                        AequusWorld.RandomUpdateTile(i, j, checkNPCSpawns: false);
                    }
                }
            }

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    var cache = map[i, j];
                    int x = i + map.Area.X;
                    int y = j + map.Area.Y;
                    if (cache.HasTile != Main.tile[x, y].HasTile || cache.TileType != Main.tile[x, y].TileType
                         || cache.Slope != Main.tile[x, y].Slope || cache.IsHalfBlock != Main.tile[x, y].IsHalfBlock)
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            Vector2 spawnLoc = Vector2.Zero;
                            var spawnVelocity = new Vector2(0f, -1f);
                            if (!Main.tile[x, y + 1].HasTile || !Main.tile[x, y - 1].HasTile || !Main.tile[x + 1, y].HasTile || !Main.tile[x - 1, y].HasTile)
                            {
                                if (Main.tile[x + 1, y].HasTile && Main.rand.NextBool())
                                {
                                    spawnLoc = new Vector2(x * 16f + 16f, y * 16f + Main.rand.NextFloat(16f));
                                    spawnVelocity = new Vector2(-1f, 0f);
                                }
                                if (Main.tile[x - 1, y].HasTile && Main.rand.NextBool())
                                {
                                    spawnLoc = new Vector2(x * 16f, y * 16f + Main.rand.NextFloat(16f));
                                    spawnVelocity = new Vector2(1f, 0f);
                                }
                                if (Main.tile[x, y - 1].HasTile && Main.rand.NextBool())
                                {
                                    spawnLoc = new Vector2(x * 16f + Main.rand.NextFloat(16f), y * 16f);
                                    spawnVelocity = new Vector2(0f, 1f);
                                }
                                if (Main.tile[x, y + 1].HasTile && (Main.rand.NextBool() || spawnLoc == Vector2.Zero))
                                {
                                    spawnLoc = new Vector2(x * 16f + Main.rand.NextFloat(16f), y * 16f + 16f);
                                    spawnVelocity = new Vector2(0f, -1f);
                                }
                            }
                            if (spawnLoc == Vector2.Zero)
                            {
                                spawnLoc = new Vector2(x * 16f + Main.rand.NextFloat(16f), y * 16f + 16f);
                            }
                            var color = ChooseDecentColorForGrownTile(Main.tile[x, y], cache);
                            var d = Dust.NewDustPerfect(spawnLoc, ModContent.DustType<MonoDust>(), newColor: color, Scale: Main.rand.NextFloat(0.8f, 1.8f));
                            d.velocity *= 0.2f;
                            d.velocity += (d.position - Projectile.Center) / 32f;
                            d.velocity += spawnVelocity;
                        }
                    }
                }
            }
        }

        public Color ChooseDecentColorForGrownTile(Tile tile, TileDataCache cache)
        {
            switch (tile.TileType)
            {
                case TileID.Crystals:
                    return new Color(255, 50, 200, 120);
                case TileID.Stalactite:
                    return new Color(120, 200, 255, 120);
            }
            if (TileID.Sets.Hallow[tile.TileType])
            {
                return new Color(255, 50, 200, 120);
            }
            if (TileID.Sets.Crimson[tile.TileType])
            {
                return new Color(255, 80, 60, 120);
            }
            if (TileID.Sets.Corrupt[tile.TileType])
            {
                return new Color(200, 50, 255, 120);
            }
            return new Color(50, 255, 100, 120);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}