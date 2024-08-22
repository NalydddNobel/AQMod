using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Common.World;

public class TilesInWorld : ModSystem {
    public bool[] Found = [];

    public override void ClearWorld() {
        Found = new bool[TileLoader.TileCount];
    }

    const string Tag_Vanilla = "VanillaTiles";
    const string Tag_Modded = "ModdedTiles";

    public override void SaveWorldData(TagCompound tag) {
        tag[Tag_Vanilla] = Helper.ConvertToByte(new BitArray(Found[..TileID.Count]));
        List<string> moddedTiles = [];
        for (int i = TileID.Count; i < TileLoader.TileCount; i++) {
            if (Found[i]) {
                moddedTiles.Add(TileLoader.GetTile(i).FullName);
            }
        }
        if (moddedTiles.Count > 0) {
            tag[Tag_Modded] = moddedTiles;
        }
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet(Tag_Vanilla, out byte[] vanillaTiles)) {
            BitArray bitArray = new BitArray(vanillaTiles);

            int length = Math.Min(bitArray.Length, TileID.Count);
            for (int i = 0; i < length; i++) {
                Found[i] = bitArray[i];
            }
        }

        if (tag.TryGet(Tag_Modded, out List<string> moddedTiles)) {
            foreach (string moddedTile in moddedTiles) {
                if (ModContent.TryFind(moddedTile, out ModTile modTile)) {
                    Found[modTile.Type] = true;
                }
            }
        }
    }
}

class TilesInWorldGlobalTile : GlobalTile {
    public override void RandomUpdate(int i, int j, int type) {
        NotifyFound(type);
        ModContent.GetInstance<TilesInWorld>().Found[type] = true;
    }

    [Conditional("DEBUG")]
    static void NotifyFound(int type) {
        if (!ModContent.GetInstance<TilesInWorld>().Found[type]) {
            LocalizedText localizedName = Lang._mapLegendCache.FromType(type);
            string name = localizedName.Value == localizedName.Key ? TileID.Search.GetName(type) : localizedName.Value;

            Main.NewText($"Found \"{name}\"");
        }
    }
}