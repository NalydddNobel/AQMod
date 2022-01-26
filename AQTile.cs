using AQMod.Common;
using AQMod.Content.World;
using AQMod.Tiles.Nature;
using AQMod.Tiles.Nature.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQTile : GlobalTile
    {
        public override void RandomUpdate(int i, int j, int type)
        {
            if (Main.tile[i, j] == null)
                Main.tile[i, j] = new Tile();
            if (WorldGen.genRand.NextBool(10000) && GoreNest.GrowGoreNest(i, j, true, true))
                return;
            switch (type)
            {
                case TileID.Stone:
                    if (j > Main.rockLayer && WorldGen.genRand.NextBool(300))
                    {
                        if (NobleMushroomsNew.Place(i, j))
                            return;
                    }
                    break;
            }
            if (WorldGen.genRand.NextBool(3000) && j > 200 && !Main.tile[i, j].active() && Framing.GetTileSafely(i, j + 1).active()
                && Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j].liquid > 0 && !Main.tile[i, j].lava() && !Main.tile[i, j].honey())
            {
                Main.tile[i, j].active(active: true);
                Main.tile[i, j].halfBrick(halfBrick: false);
                Main.tile[i, j].slope(slope: 0);
                Main.tile[i, j].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                Main.tile[i, j].frameX = (short)(22 * ExoticCoralNew.GetRandomStyle(WorldGen.genRand.Next(3)));
                Main.tile[i, j].frameY = 0;
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
                        return true;
                }
            }
            return false;
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            switch (type)
            {
                case TileID.ShadowOrbs:
                    return !ModContent.GetInstance<AQConfigServer>().evilProgressionLock || WorldDefeats.AnyBossDefeated() || Main.LocalPlayer.HeldItem.hammer >= 60;
            }
            if (j > 1)
            {
                if (ProtectedTile(i, j - 1))
                    return false;
            }
            return true;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            switch (type)
            {
                case TileID.Ebonstone:
                case TileID.Crimstone:
                    return !ModContent.GetInstance<AQConfigServer>().evilProgressionLock || WorldDefeats.AnyBossDefeated() || j < 400;

                case TileID.ShadowOrbs:
                    return !ModContent.GetInstance<AQConfigServer>().evilProgressionLock || WorldDefeats.AnyBossDefeated();
            }
            if (j > 1)
            {
                if (ProtectedTile(i, j - 1))
                    return false;
            }
            return true;
        }

        public override bool Slope(int i, int j, int type)
        {
            if (j > 1)
            {
                if (ProtectedTile(i, j - 1))
                    return false;
            }
            return true;
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Main.netMode != NetmodeID.Server && !Main.gameMenu && !WorldGen.noTileActions 
                && VeinmineWorldData.CanVeinmineAtAll(type) && !fail && !effectOnly)
            {
                try
                {
                    if (Main.player[Main.myPlayer].HeldItem.pick > 0 && Player.tileTargetX == i && Player.tileTargetY == j 
                        && Main.player[Main.myPlayer].GetModPlayer<AQPlayer>().VeinmineTiles[type])
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            VeinmineWorldData.AddUpdateTarget(i, j, VeinmineWorldData.veinmineReps);
                        }
                        else
                        {
                            NetHelper.RequestVeinmine(i, j, Main.myPlayer);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public static class WindFXHelper
        {
            public static bool WindBlocked(int i, int j)
            {
                return WindBlocked(Main.tile[i, j]);
            }
            public static bool WindBlocked(Tile tile)
            {
                return tile.wall != 0;
            }
        }
    }
}