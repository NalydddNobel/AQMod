using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

public partial class AequusTile : GlobalTile, IPostSetupContent {
    private static void SetCollision<T>(bool value, out bool oldValue) where T : ModTile {
        int tileID = ModContent.TileType<T>();
        oldValue = Main.tileSolid[tileID];
        SetCollision<T>(value);
    }

    private static void SetCollision<T>(bool value) where T : ModTile {
        Main.tileSolid[ModContent.TileType<T>()] = value;
    }

    private static void WorldGen_QuickFindHome(On_WorldGen.orig_QuickFindHome orig, int npc) {
        //SetCollision<EmancipationGrill>(value: true, out bool emancipationGrill);
        orig(npc);
        //SetCollision<EmancipationGrill>(value: emancipationGrill);
    }
}