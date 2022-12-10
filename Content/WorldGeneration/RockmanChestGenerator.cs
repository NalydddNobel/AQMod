using Aequus.Items.Weapons.Melee;
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
            for (int k = 0; k < 10000; k++)
            {
                var r = Utils.CenteredRectangle(new Vector2(WorldGen.genRand.Next(100, Main.maxTilesX - 100), WorldGen.genRand.Next((int)Main.worldSurface + 150, (int)Main.worldSurface + 500)), 
                    new Vector2(WorldGen.genRand.Next(Main.maxTilesX / (AequusWorld.SmallWidth / 100), Main.maxTilesX / (AequusWorld.SmallWidth / 200)))).Fluffize(100);
                if (WorldGen.structures?.CanPlace(r, AequusTile.All) == true)
                    continue;
                GrowGrass(r);
                AequusWorld.Structures.Add("Rockman", r.Center);
                for (int l = 0; l < 100000; l++)
                {
                    var p = WorldGen.genRand.NextFromRect(r).ToPoint();
                    int chestID = TryPlaceChest(p.X, p.Y);
                    if (chestID != -1)
                    {
                        FillChest(chestID);
                        break;
                    }
                }
                break;
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
                    if (new Vector2(i, j).Distance(v) < size)
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
                                        for (int m = 0; m < 50; m++)
                                        {
                                            WorldGen.GrowTree(i + k, j + l - 1);
                                        }

                                        t.TileType = TileID.Grass;
                                        break;
                                    }
                                }
                            }
                        }
                        WorldGen.SquareTileFrame(i, j);
                        if (t.WallType > WallID.None)
                        {
                            t.WallType = WallID.FlowerUnsafe;
                        }
                    }
                }
            }
        }

        public int TryPlaceChest(int x, int y)
        {
            return WorldGen.PlaceChest(x, y, 21, style: ChestType.LockedGold);
        }

        public void FillChest(int chestID)
        {
            Main.chest[chestID].item[0].SetDefaults(ModContent.ItemType<RockMan>());
        }
    }
}