using Aequu2.DataSets;

namespace Aequu2.Old.Content.Tiles.Herbs;

public class StaffOfRegrowthHelper : ModSystem {
    public override void Load() {
        On_Player.PlaceThing_Tiles_BlockPlacementForAssortedThings += Player_PlaceThing_Tiles_BlockPlacementForAssortedThings;
    }

    private static bool Player_PlaceThing_Tiles_BlockPlacementForAssortedThings(On_Player.orig_PlaceThing_Tiles_BlockPlacementForAssortedThings orig, Player player, bool canPlace) {
        int i = Player.tileTargetX;
        int j = Player.tileTargetY;
        Tile tile = Main.tile[i, j];

        if (ItemDataSet.IsStaffOfRegrowth.Contains(player.HeldItem.type) && tile.HasTile && tile.TileType >= TileID.Count && TileLoader.GetTile(tile.TileType) is ModHerb herbTile) {
            if (herbTile.GetGrowthStage(i, j) == ModHerb.STAGE_BLOOMING) {
                WorldGen.KillTile(i, j);
                if (!Main.tile[i, j].HasTile && Main.netMode == NetmodeID.MultiplayerClient) {
                    NetMessage.SendData(MessageID.TileManipulation, number: 0, number2: i, number3: j);
                }
            }

            canPlace = true;
        }

        return orig(player, canPlace);
    }
}
