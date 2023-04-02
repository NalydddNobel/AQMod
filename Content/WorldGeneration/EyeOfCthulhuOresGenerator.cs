using Terraria;
using Terraria.ID;

namespace Aequus.Content.WorldGeneration
{
    public class EyeOfCthulhuOresGenerator : Generator
    {
        protected override void Generate()
        {
            int[] ore = DetermineOres();
            var rand = Rand;
            for (int j = (int)Main.rockLayer; j < Main.UnderworldLayer; j += 175)
            {
                for (int i = 0; i < Main.maxTilesX; i += 70)
                {
                    for (int k = 0; k < 50; k++)
                    {
                        int x = i + WorldGen.genRand.Next(-40, 40);
                        int y = j + WorldGen.genRand.Next(-40, 40);
                        if (!WorldGen.InWorld(x, y, fluff: 40) || (!TileID.Sets.Stone[Main.tile[x, y].TileType] && !TileID.Sets.Mud[Main.tile[x, y].TileType]))
                        {
                            continue;
                        }
                        WorldGen.TileRunner(x, y, rand.Next(3, 7), rand.Next(10, 40), (ushort)rand.Next(ore), addTile: true);
                        for (int m = -20; m < 20; m++)
                        {
                            for (int n = -20; n < 20; n++)
                            {
                                if (WorldGen.InWorld(x + m, y + n, fluff: 40) && ore.ContainsAny(Main.tile[x + m, y + n].TileType))
                                {
                                    var t = Main.tile[x + m, y + n];
                                    t.Slope = SlopeType.Solid;
                                    t.HalfBrick(value: false);
                                    WorldGen.SquareTileFrame(x + m, y + n);
                                    if (Main.netMode != NetmodeID.SinglePlayer) {
                                        NetMessage.SendTileSquare(-1, x + m, y + n);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        public int[] DetermineOres()
        {
            return Main.drunkWorld ? (new int[] { TileID.Crimtane, TileID.Demonite })
                : WorldGen.crimson ? new int[] { TileID.Crimtane, } : new int[] { TileID.Demonite, };
        }

        public string GetMessage()
        {
            return Main.drunkWorld ? "Announcement.EyeOfCthulhuDrunk"
                : WorldGen.crimson ? "Announcement.EyeOfCthulhuCrimtane" : "Announcement.EyeOfCthulhuDemonite";
        }
    }
}