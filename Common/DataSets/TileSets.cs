using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets {
    public class TileSets : DataSet {
        public static readonly HashSet<int> PreventsSlopesBelow = new();
        public static int[] TileRenderConversion;

        public override void OnLoad(Mod mod) {
            TileRenderConversion = new int[0]; 
        }

        public override void PostSetupContent() {
            Array.Resize(ref TileRenderConversion, TileLoader.TileCount);
        }

        public static void AddTileRenderConversion(int tileType, int tileConversion) {
            if (tileType >= TileRenderConversion.Length) {
                Array.Resize(ref TileRenderConversion, tileType + 1);
            }
            TileRenderConversion[tileType] = tileConversion;
        }
    }
}