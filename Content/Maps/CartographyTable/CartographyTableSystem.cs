using Terraria.Map;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Maps.CartographyTable;

public class CartographyTableSystem : ModSystem {
    public static CartographyTableSystem Instance => ModContent.GetInstance<CartographyTableSystem>();

    public ServerMap? Map { get; private set; }

    public override void Load() {
        On_WorldMap.UpdateLighting += On_WorldMap_UpdateLighting;
    }

    private static bool On_WorldMap_UpdateLighting(On_WorldMap.orig_UpdateLighting orig, WorldMap self, int x, int y, byte light) {
        bool result = orig(self, x, y, light);
        if (result) {
            SendResult(Main.Map[x, y]);
        }
        return result;

        void SendResult(MapTile tile) {
            if (Main.netMode == NetmodeID.MultiplayerClient) {

            }
            else {
                ModContent.GetInstance<CartographyTableSystem>().Map!.SetLight(x, y, tile.Light);
            }
        }
    }

    public override void ClearWorld() {
        Map = new ServerMap(Main.maxTilesX, Main.maxTilesY);
    }

    public override void SaveWorldData(TagCompound tag) {
        tag["ServerMap"] = Map!.Save();
    }

    public override void LoadWorldData(TagCompound tag) {
        Map = new ServerMap(Main.maxTilesX, Main.maxTilesY);
        if (tag.TryGet("ServerMap", out TagCompound loadedMap)) {
            Map!.Load(loadedMap);
        }
    }
}
