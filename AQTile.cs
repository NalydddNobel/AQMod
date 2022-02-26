using AQMod.Common;
using AQMod.Common.ID;
using AQMod.Tiles.ExporterQuest;
using AQMod.Tiles.Nature;
using AQMod.Tiles.Nature.CrabCrevice;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQTile : GlobalTile
    {
        internal const byte Slope_BottomLeft = 1;
        internal const byte Slope_BottomRight = 2;
        internal const byte Slope_TopLeft = 3;
        internal const byte Slope_TopRight = 4;

        public static class Sets
        {
            public static HashSet<ushort> CanFixWaterOnType { get; private set; }
            public static HashSet<ushort> ExporterQuestFurniture { get; private set; }

            internal static void Setup()
            {
                CanFixWaterOnType = new HashSet<ushort>()
                {
                    TileID.Grass, 
                    TileID.CorruptGrass, 
                    TileID.FleshGrass, 
                    TileID.SnowBlock, 
                    TileID.IceBlock, 
                    TileID.Sand,
                };

                ExporterQuestFurniture = new HashSet<ushort>()
                {
                    (ushort)ModContent.TileType<JeweledCandelabraTile>(),
                    (ushort)ModContent.TileType<JeweledChaliceTile>(),
                };
            }

            internal static void Unload()
            {
                CanFixWaterOnType?.Clear();
                CanFixWaterOnType = null;
                ExporterQuestFurniture?.Clear();
                ExporterQuestFurniture = null;
            }
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            if (Main.tile[i, j] == null)
                Main.tile[i, j] = new Tile();
            if (WorldGen.genRand.NextBool(10000) && GoreNest.TryGrowGoreNest(i, j, true, true))
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