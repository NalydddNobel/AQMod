using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.World
{
    public sealed class VeinmineWorldData : ModWorld
    {
        public static List<(ushort, ushort, byte)> VeinmineUpdateTargets { get; private set; }
        public static byte veinmineReps = 25;

        public override void Initialize()
        {
            VeinmineUpdateTargets = new List<(ushort, ushort, byte)>();
        }

        public override void PostUpdate()
        {
            if (VeinmineUpdateTargets == null)
            {
                VeinmineUpdateTargets = new List<(ushort, ushort, byte)>();
            }
            if (VeinmineUpdateTargets.Count == 0)
                return;
            var currentList = new (ushort, ushort, byte)[VeinmineUpdateTargets.Count];
            VeinmineUpdateTargets.CopyTo(currentList);
            VeinmineUpdateTargets.Clear();
            foreach (var t in currentList)
            {
                InternalExpandQuarry(t.Item1, t.Item2, t.Item3, Main.tile[t.Item1, t.Item2].type);
            }
            foreach (var t in VeinmineUpdateTargets)
            {
                WorldGen.KillTile(t.Item1, t.Item2, fail: false, effectOnly: false, noItem: false);
            }
        }

        public static void AddUpdateTarget(int i, int j, byte reps)
        {
            AddUpdateTarget((ushort)i, (ushort)j, reps);
        }

        public static void AddUpdateTarget(ushort i, ushort j, byte reps)
        {
            InternalExpandQuarry(i, j, reps, Main.tile[i, j].type);
        }

        public static bool CanVeinmineAtAll(int i, int j)
        {
            return CanVeinmineAtAll(Main.tile[i, j].type);
        }

        public static bool CanVeinmineAtAll(Tile tile)
        {
            return CanVeinmineAtAll(tile.type);
        }

        public static bool CanVeinmineAtAll(int type)
        {
            return !Main.tileFrameImportant[type];
        }

        private static bool InternalExpandQuarry(ushort x, ushort y, byte index, int type)
        {
            if (index == 0)
                return false;

            InternalExpand(x, (ushort)(y - 1), index, type);
            InternalExpand(x, (ushort)(y + 1), index, type);
            InternalExpand((ushort)(x - 1), y, index, type);
            InternalExpand((ushort)(x + 1), y, index, type);
            return true;
        }

        private static void InternalExpand(ushort x, ushort y, byte index, int type)
        {
            if (Framing.GetTileSafely(x, y).active() && Main.tile[x, y].type == type)
                VeinmineUpdateTargets.Add((x, y, (byte)(index - 1)));
        }
    }
}