﻿using Aequus.Common.World;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.World.Generation {
    public class EOCOresGenerator : Generator {
        protected override void Generate() {
            int[] ore = DetermineOres();
            var rand = Rand;
            for (int j = (int)Main.rockLayer; j < Main.UnderworldLayer; j += 175) {
                for (int i = 0; i < Main.maxTilesX; i += 70) {
                    if (TileHelper.ScanTilesSquare(i, j, 400, TileHelper.HasShimmer)) {
                        continue;
                    }
                    for (int k = 0; k < 50; k++) {
                        int x = i + rand.Next(-40, 40);
                        int y = j + rand.Next(-40, 40);

                        if (!WorldGen.InWorld(x, y, fluff: 40) || !TileID.Sets.Stone[Main.tile[x, y].TileType] || TileHelper.ScanTilesSquare(x, y, 60, TileHelper.HasMinecartRail, TileHelper.HasContainer, TileHelper.HasImportantTile)) {
                            continue;
                        }

                        WorldGen.TileRunner(x, y, rand.Next(3, 7), rand.Next(10, 20), (ushort)rand.Next(ore), addTile: true);

                        for (int m = -20; m < 20; m++) {
                            for (int n = -20; n < 20; n++) {
                                if (WorldGen.InWorld(x + m, y + n, fluff: 40) && ore.ContainsAny(Main.tile[x + m, y + n].TileType)) {
                                    var t = Main.tile[x + m, y + n];
                                    t.Slope = SlopeType.Solid;
                                    t.HalfBrick(value: false);
                                    AequusWorldGenerator.tileFrameLoop = 99;
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

        public int[] DetermineOres() {
            return Main.drunkWorld ? (new int[] { TileID.Crimtane, TileID.Demonite })
                : WorldGen.crimson ? new int[] { TileID.Crimtane, } : new int[] { TileID.Demonite, };
        }

        public string GetMessage() {
            return Main.drunkWorld ? "Announcement.EyeOfCthulhuDrunk"
                : WorldGen.crimson ? "Announcement.EyeOfCthulhuCrimtane" : "Announcement.EyeOfCthulhuDemonite";
        }
    }
}