using AQMod.Common.WorldGeneration;
using AQMod.Content;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class AQTile : GlobalTile
    {
        public override void RandomUpdate(int i, int j, int type)
        {
            if (Main.tile[i, j] == null)
            {
                Main.tile[i, j] = new Tile();
            }
            if (WorldGen.genRand.NextBool(10000) && GoreNest.GrowGoreNest(i, j, true, true))
            {
                return;
            }
            switch (type)
            {
                case TileID.Stone:
                if (j > Main.rockLayer && WorldGen.genRand.NextBool(300))
                {
                    if (PlaceRandomNobleMushroom(i, j))
                        return;
                }
                break;
            }
            if (WorldGen.genRand.NextBool(3000) && j > 200 && !Main.tile[i, j].active() && Framing.GetTileSafely(i, j + 1).active() && Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j].liquid > 0 && !Main.tile[i, j].lava() && !Main.tile[i, j].honey())
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<ExoticCoral>(), true, false, -1, WorldGen.genRand.Next(3));
                return;
            }
        }

        public static bool ProtectedTile(int i, int j)
        {
            var tile = Main.tile[i, j];
            if (tile.type > Main.maxTileSets && Main.tileFrameImportant[tile.type])
            {
                bool blockDamaged = false;
                if (TileLoader.GetTile(tile.type).mod.Name == "AQMod")
                {
                    if (!TileLoader.CanKillTile(i, j, tile.type, ref blockDamaged))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            switch (type)
            {
                case TileID.ShadowOrbs:
                return !AQMod.EvilProgressionLock || AQMod.AnyBossDefeated() || Main.LocalPlayer.HeldItem.hammer >= 60;
            }
            if (!Main.tileFrameImportant[type] && Main.tileSolid[type])
            {
                if (ProtectedTile(i, j - 1))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            switch (type)
            {
                case TileID.Ebonstone:
                case TileID.Crimstone:
                return !AQMod.EvilProgressionLock || AQMod.AnyBossDefeated() || j < 400;

                case TileID.ShadowOrbs:
                return !AQMod.EvilProgressionLock || AQMod.AnyBossDefeated();
            }
            if (!Main.tileFrameImportant[type] && Main.tileSolid[type])
            {
                if (ProtectedTile(i, j - 1))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Slope(int i, int j, int type)
        {
            if (!Main.tileFrameImportant[type] && Main.tileSolid[type])
            {
                if (ProtectedTile(i, j - 1))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool _veinmine;
        private static uint _veinmineUpdateDelay;
        private static bool veinmineUseStopwatch => Main.netMode == NetmodeID.SinglePlayer;

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.GameUpdateCount >= _veinmineUpdateDelay && VeinmineHelper.CanVeinmineAtAll(type) && !fail && !effectOnly && !_veinmine)
            {
                byte plr = Player.FindClosest(new Vector2(i * 16f, j * 16f), 16, 16);
                if (Main.player[plr].GetModPlayer<AQPlayer>().veinmineTiles[type] && Player.tileTargetX == i && Player.tileTargetY == j && Main.player[plr].HeldItem.pick > 0)
                {
                    Stopwatch stopwatch = null;

                    if (veinmineUseStopwatch)
                    {
                        stopwatch = new Stopwatch();
                        stopwatch.Start();
                    }

                    noItem = true;
                    _veinmine = true;
                    VeinmineHelper.VeinmineTile(i, j, Main.player[plr]);
                    _veinmine = false;

                    if (veinmineUseStopwatch)
                    {
                        ModContent.GetInstance<AQMod>().Logger.Debug((uint)stopwatch.ElapsedMilliseconds * 10);
                        _veinmineUpdateDelay = Main.GameUpdateCount + (uint)stopwatch.ElapsedMilliseconds * 10;
                        stopwatch.Stop();
                    }
                    else
                    {
                        _veinmineUpdateDelay = Main.GameUpdateCount + 10;
                    }
                }
            }
        }

        internal static bool PlaceRandomNobleMushroom(int x, int y)
        {
            if (AQWorldGen.check2x2(x, y - 1))
            {
                int style = WorldGen.genRand.Next(3);
                WorldGen.PlaceTile(x, y - 1, ModContent.TileType<NobleMushrooms>(), true, true, -1, style);
                return true;
            }
            return false;
        }
    }
}