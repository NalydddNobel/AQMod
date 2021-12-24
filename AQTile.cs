using AQMod.Common.WorldGeneration;
using AQMod.Content;
using AQMod.Tiles;
using AQMod.Tiles.Nature;
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
                return !ModContent.GetInstance<AQConfigServer>().evilProgressionLock || AQMod.AnyBossDefeated() || Main.LocalPlayer.HeldItem.hammer >= 60;
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
                return !AQMod.EvilProgressionLock || AQMod.AnyBossDefeated() || j < 400;

                case TileID.ShadowOrbs:
                return !AQMod.EvilProgressionLock || AQMod.AnyBossDefeated();
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

        private static bool _veinmine;
        private static uint _veinmineUpdateDelay;

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!Main.gameMenu && Main.netMode != NetmodeID.Server && !WorldGen.noTileActions && 
                Main.GameUpdateCount >= _veinmineUpdateDelay && VeinmineHelper.CanVeinmineAtAll(type) && !fail && !effectOnly && !_veinmine)
            {
                try
                {
                    if (Main.player[Main.myPlayer].HeldItem.pick > 0 && Player.tileTargetX == i && Player.tileTargetY == j && Main.player[Main.myPlayer].GetModPlayer<AQPlayer>().veinmineTiles[type])
                    {
                        noItem = true;
                        _veinmine = true;
                        VeinmineHelper.VeinmineTile(i, j, Main.player[Main.myPlayer]);
                        _veinmine = false;
                        _veinmineUpdateDelay = Main.GameUpdateCount + 10;
                    }
                }
                catch
                {

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