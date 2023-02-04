﻿using Aequus.Items.Weapons.Melee;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.WorldGeneration
{
    public class RockmanChestGenerator
    {
        public void GenerateRandomLocation()
        {
            int spawnedCount = 0;
            int amt = Main.maxTilesX / (AequusWorld.SmallWidth / 2);
            var invalidTiles = new bool[TileLoader.TileCount];
            TileID.Sets.GeneralPlacementTiles.CopyTo(invalidTiles, 0);
            invalidTiles[TileID.MushroomGrass] = false;
            invalidTiles[TileID.MushroomBlock] = false;
            invalidTiles[TileID.MarbleBlock] = false;
            invalidTiles[TileID.Marble] = false;
            invalidTiles[TileID.GraniteBlock] = false;
            invalidTiles[TileID.Granite] = false;
            for (int i = 0; i < TileLoader.TileCount; i++)
            {
                if (Main.tileSand[i] || TileID.Sets.IcesSnow[i] || TileID.Sets.Corrupt[i] || TileID.Sets.Crimson[i] || TileID.Sets.Hallow[i])
                {
                    invalidTiles[i] = false;
                }
            }
            for (int k = 0; k < 100000 && spawnedCount < amt; k++)
            {
                var areaForGenerating = Utils.CenteredRectangle(new Vector2(WorldGen.genRand.Next(100, Main.maxTilesX - 100), WorldGen.genRand.Next((int)Main.worldSurface + 150, (int)Main.worldSurface + 500)),
                    new Vector2(WorldGen.genRand.Next(Main.maxTilesX / (AequusWorld.SmallWidth / 80), Main.maxTilesX / (AequusWorld.SmallWidth / 120)))).Fluffize(100);

                if (WorldGen.structures?.CanPlace(areaForGenerating, invalidTiles, 8) == false)
                    continue;

                WorldGen.structures.AddStructure(areaForGenerating);
                GrowGrass(areaForGenerating);
                AequusWorld.Structures.Add($"Rockman_{spawnedCount}", areaForGenerating.Center);
                spawnedCount++;
                var v = new Vector2(areaForGenerating.X + areaForGenerating.Width / 2f, areaForGenerating.Y + areaForGenerating.Height / 2f);
                float size = new Vector2(areaForGenerating.Width, areaForGenerating.Height).Length() / MathHelper.Pi;
                for (int l = 0; l < 100000; l++)
                {
                    var p = WorldGen.genRand.NextFromRect(areaForGenerating).ToPoint();
                    if (p.ToVector2().Distance(v) > size)
                    {
                        continue;
                    }
                    int chestID = TryPlaceChest(p.X, p.Y);
                    if (chestID != -1)
                    {
                        FillChest(chestID);
                        break;
                    }
                }
            }
        }

        public void GrowGrass(Rectangle dimensions)
        {
            var v = new Vector2(dimensions.X + dimensions.Width / 2f, dimensions.Y + dimensions.Height / 2f);
            float size = new Vector2(dimensions.Width, dimensions.Height).Length() / MathHelper.Pi;
            for (int i = dimensions.X; i < dimensions.X + dimensions.Width; i++)
            {
                for (int j = dimensions.Y; j < dimensions.Y + dimensions.Height; j++)
                {
                    if (new Vector2(i, j).Distance(v) + WorldGen.genRand.Next(-10, 0) < size)
                    {
                        var t = Main.tile[i, j];
                        if (t.IsFullySolid())
                        {
                            t.TileType = TileID.Dirt;
                            for (int k = -1; k <= 1; k++)
                            {
                                for (int l = -1; l <= 1; l++)
                                {
                                    if (!Main.tile[i + k, j + l].IsFullySolid())
                                    {
                                        t.TileType = TileID.Grass;
                                        if (!Main.tile[i, j - 1].IsFullySolid())
                                        {
                                            WorldGen.KillTile(i, j - 1);
                                            WorldGen.PlaceTile(i, j - 1, TileID.Saplings, style: 0);
                                            for (int m = 0; m < 50 && !TileID.Sets.IsATreeTrunk[Main.tile[i, j - 1].TileType]; m++)
                                            {
                                                WorldGen.GrowTree(i, j - 1);
                                            }
                                            if (!TileID.Sets.IsATreeTrunk[Main.tile[i, j - 1].TileType])
                                            {
                                                WorldGen.KillTile(i, j - 1);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        WorldGen.SquareTileFrame(i, j);
                        if (t.WallType != WallID.HiveUnsafe && t.WallType != WallID.LihzahrdBrickUnsafe && !Main.wallDungeon[t.WallType])
                        {
                            t.WallType = WallID.FlowerUnsafe;
                        }
                    }
                }
            }
        }

        public int TryPlaceChest(int x, int y)
        {
            if (Main.tile[x, y].LiquidAmount > 0)
                return -1;
            return WorldGen.PlaceChest(x, y, TileID.Containers, style: ChestType.LockedGold);
        }

        public void FillChest(int chestID)
        {
            Main.chest[chestID].Insert(ModContent.ItemType<RockMan>(), 0);
        }
    }
}