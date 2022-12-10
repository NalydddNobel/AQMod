using Aequus.Items.GlobalItems;
using Aequus.Tiles;
using Aequus.Tiles.Ambience;
using Aequus.Tiles.Furniture;
using Aequus.Tiles.Furniture.Oblivion;
using Aequus.Tiles.Misc;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.WorldGeneration
{
    public class GoreNestGenerator
    {
        public int min;
        public int max;
        public int MinY => Main.UnderworldLayer + min;
        public int MaxY => Main.UnderworldLayer + max;

        public HashSet<int> AvoidTiles { get; private set; }
        public HashSet<int> AvoidWalls { get; private set; }

        public GoreNestGenerator()
        {
            min = 75;
            max = 150;
            AvoidTiles = new HashSet<int>()
            {
                TileID.ObsidianBrick,
                TileID.HellstoneBrick,
            };
            AvoidWalls = new HashSet<int>()
            {
                WallID.ObsidianBrick,
                WallID.ObsidianBrickUnsafe,
                TileID.HellstoneBrick,
            };
        }

        public void Generate()
        {
            int goreNestCount = 0;
            for (int i = 0; i < (goreNestCount == 0 ? 1000000 : 10000); i++)
            {
                int x = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
                int y = WorldGen.genRand.Next(MinY, MaxY);
                if (TryGrowGoreNest(x, y))
                {
                    goreNestCount++;
                    if (goreNestCount > Main.maxTilesX / 1500)
                        break;
                }
            }
        }

        public bool TryGrowGoreNest(int x, int y)
        {
            if (Main.tile[x, y].HasTile)
            {
                if (!Main.tile[x, y - 1].HasTile)
                {
                    y--;
                }
                else
                {
                    return false;
                }
            }
            else if (!Main.tile[x, y + 1].HasTile)
            {
                return false;
            }
            var structure = new Rectangle(x - 60, y - 60, 120, 90).Fluffize(5);
            if (!WorldGen.structures.CanPlace(structure, AequusTile.All) || CheckForBlacklistedTiles(x, y))
                return false;
            y -= 2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x2 = x + i;
                    int y2 = y + j;
                    Main.tile[x2, y2].Active(value: false);
                    if (Main.tile[x2, y2].HasTile || Main.tile[x2, y2].LiquidAmount > 0)
                        return false;
                }
            }
            y += 3;
            for (int i = 0; i < 3; i++)
            {
                int x2 = x + i;
                if (!Main.tile[x2, y].HasTile || !Main.tileSolid[Main.tile[x2, y].TileType] || Main.tileCut[Main.tile[x2, y].TileType])
                    return false;
            }
            for (int i = 0; i < 3; i++)
            {
                int x2 = x + i;
                Main.tile[x2, y].Slope(value: 0);
                Main.tile[x2, y].HalfBrick(value: false);
            }
            y--;
            x++;
            WorldGen.PlaceTile(x, y, ModContent.TileType<GoreNestTile>(), mute: true, forced: true);
            if (Main.tile[x, y].TileType != ModContent.TileType<GoreNestTile>())
            {
                return false;
            }
            GenerateSurroundingGoreNestHill(x, y);
            GenerateChests(x, y);
            GenerateSigns(x, y);
            GenerateAmbientTiles(x, y);
            WorldGen.structures.AddStructure(structure);
            return true;
        }
        public bool CheckForBlacklistedTiles(int x, int y)
        {
            for (int i = x - 25; i < x + 25; i++)
            {
                for (int j = y - 25; j < y + 25; j++)
                {
                    if (Main.tile[i, j].HasTile && AvoidTiles.Contains(Main.tile[i, j].TileType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void GenerateSurroundingGoreNestHill(int x, int y)
        {
            HillSpawnAsh(x, y);
            int k = 0;
            while (y < MaxY)
            {
                if (WorldGen.genRand.NextBool(3))
                {
                    y++;
                }

                HillSpawnAsh(x + k + 1, y);
                HillSpawnAsh(x - k - 1, y);
                k++;
                if (k > 45 || (k > 20 && WorldGen.genRand.NextBool(15)))
                {
                    break;
                }
            }

            HillTryToSmoothyGoIntoRegularGeneration(x, y, k, 1);
            HillTryToSmoothyGoIntoRegularGeneration(x, y, k, -1);
        }
        public void HillTryToSmoothyGoIntoRegularGeneration(int x, int y, int k, int dir)
        {
            k *= dir;
            if (y < Main.maxTilesY)
            {
                y = Main.maxTilesY - 1;
            }
            while ((x + k + dir < 0 || x + k + dir > Main.maxTilesX)
                && !Main.tile[x + k + dir, y].HasTile && !Main.tile[x + k + dir, y - 1].HasTile)
            {
                k += dir;
                if (WorldGen.genRand.NextBool(3))
                {
                    y--;
                }
                if (y < Main.UnderworldLayer)
                {
                    break;
                }
                HillSpawnAsh(x + k, y, kill: true);
            }
            x -= dir;
            while (true)
            {
                x += dir;
                if (x + k < 0 || x + k > Main.maxTilesX)
                {
                    break;
                }
                y += WorldGen.genRand.Next(3);
                if (y > Main.maxTilesY)
                {
                    break;
                }
                HillSpawnAsh(x + k, y, kill: false);
            }
        }
        public void HillSpawnAsh(int x, int y, bool kill = true)
        {
            int l = 0;
            while (true)
            {
                l++;
                if (y + l > Main.maxTilesY)
                {
                    break;
                }
                if (l > 75 || (l > 40 && WorldGen.genRand.NextBool(15)))
                {
                    break;
                }

                if (kill && !AvoidTiles.Contains(Main.tile[x, y - l].TileType) && !AvoidWalls.Contains(Main.tile[x, y - l].WallType)
                    && Main.tile[x, y - l].TileType != ModContent.TileType<GoreNestTile>())
                    WorldGen.KillTile(x, y - l, noItem: true);

                Main.tile[x, y + l].LiquidAmount = 0;

                if (AvoidTiles.Contains(Main.tile[x, y + l].TileType) || AvoidWalls.Contains(Main.tile[x, y + l].WallType))
                    continue;
                WorldGen.PlaceTile(x, y + l, TileID.Ash, mute: true, forced: true);
                Main.tile[x, y + l].Slope(value: 0);
                Main.tile[x, y + l].HalfBrick(value: false);
            }
        }
        public void GenerateChests(int x, int y)
        {
            var genTangle = new Rectangle(x - 40, y - 20, 80, 40);
            for (int i = 0; i < 1250; i++)
            {
                var v = WorldGen.genRand.NextVector2FromRectangle(genTangle).ToPoint();
                if (!Main.tile[v.X, v.Y].HasTile)
                {
                    int c = WorldGen.PlaceChest(v.X, v.Y, type: (ushort)ModContent.TileType<OblivionChestTile>());
                    if (c != -1)
                    {
                        FillChest(Main.chest[c]);
                        break;
                    }
                }
            }
        }
        public void GenerateAmbientTiles(int x, int y)
        {
            var genTangle = new Rectangle(x - 40, y - 20, 80, 40);
            for (int i = 0; i < 1250; i++)
            {
                var v = WorldGen.genRand.NextVector2FromRectangle(genTangle).ToPoint();
                WorldGen.PlaceTile(v.X, v.Y, ModContent.TileType<GoreNestStalagmite>(), style: WorldGen.genRand.Next(6));
            }
        }
        public void GenerateSigns(int x, int y)
        {
            var genTangle = new Rectangle(x - 60, y - 20, 120, 40);
            for (int i = 0; i < 1250; i++)
            {
                var v = WorldGen.genRand.NextVector2FromRectangle(genTangle).ToPoint();
                if (!Main.tile[v.X, v.Y].HasTile)
                {
                    WorldGen.PlaceTile(v.X, v.Y, ModContent.TileType<Tombstones>(), style: WorldGen.genRand.Next(6));
                    if (Main.tile[v.X, v.Y].HasTile)
                    {
                        int sign = Sign.ReadSign(v.X, v.Y);
                        if (sign >= 0)
                        {
                            string text = AequusText.GetTextWith("GoreNestTombstones." + WorldGen.genRand.Next(4), new { Name = AequusText.GetText("GoreNestTombstones.Names." + WorldGen.genRand.Next(10)) });
                            Sign.TextSign(sign, text + AequusText.GetText("GoreNestTombstones.Hint." + WorldGen.genRand.Next(6)));
                        }
                        i += 400;
                    }
                }
            }
        }

        public void FillChest(Chest c)
        {
            int slot = 0;
            c.item[slot].SetDefaults(WorldGen.crimson ? ItemID.LightsBane : ItemID.BloodButcherer); // Opposite evil sword
            c.item[slot++].GetGlobalItem<ItemNameTag>().NameTag = "$Mods.Aequus.GoreNestTombstones.Names." + WorldGen.genRand.Next(11) + "|$Mods.Aequus.GoreNestTombstones.Sword";
            if (WorldGen.genRand.NextBool())
            {
                c.item[slot++].SetDefaults(Utils.SelectRandom(WorldGen.genRand, ItemID.SilverPickaxe, ItemID.TungstenPickaxe, ItemID.GoldPickaxe, ItemID.PlatinumPickaxe));
            }
            if (WorldGen.genRand.NextBool())
            {
                c.item[slot++].SetDefaults(Utils.SelectRandom(WorldGen.genRand, ItemID.SilverAxe, ItemID.TungstenAxe, ItemID.GoldAxe, ItemID.PlatinumAxe, ItemID.WarAxeoftheNight, ItemID.BloodLustCluster));
            }
            if (WorldGen.genRand.NextBool(3))
            {
                c.item[slot++].SetDefaults(Utils.SelectRandom(WorldGen.genRand, ItemID.SilverHammer, ItemID.TungstenHammer, ItemID.GoldHammer, ItemID.PlatinumHammer));
            }
            c.item[slot].SetDefaults(ItemID.HealingPotion);
            c.item[slot++].stack = WorldGen.genRand.Next(10, 25);

            if (WorldGen.genRand.NextBool(3))
            {
                c.item[slot++].SetDefaults(ItemID.ObsidianSkull);
            }

            if (WorldGen.genRand.NextBool(3))
            {
                c.item[slot++].SetDefaults(ItemID.MagicMirror);
            }
            else
            {
                int recallStack = WorldGen.genRand.Next(10);
                if (recallStack > 0)
                {
                    c.item[slot].SetDefaults(ItemID.RecallPotion);
                    c.item[slot++].stack = recallStack;
                }
            }

            if (WorldGen.genRand.NextBool())
            {
                c.item[slot++].SetDefaults(Utils.SelectRandom(WorldGen.genRand, ItemID.StarStatue, ItemID.HeartStatue, ItemID.AngelStatue));
            }
        }

        public void Cleanup()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = MinY; j < MaxY; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<GoreNestTile>())
                    {
                        CleanLava(i, j);
                    }
                }
            }
        }
        public void CleanLava(int x, int y)
        {
            for (int i = x - 60; i < x + 60; i++)
            {
                for (int j = y - 50; j < Main.maxTilesY && !Main.tile[i, j].IsSolid(); j++)
                {
                    Main.tile[i, j].LiquidAmount = 0;
                }
            }
        }
    }
}