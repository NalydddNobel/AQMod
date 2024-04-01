using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.FloatingTrash;
public abstract class FloatingTrashBase : ModTile {
    protected static bool UpdateActive(int i, int j) {
        var tile = Main.tile[i, j];
        if (tile.LiquidAmount <= 0 || Main.tile[i, j - 1].LiquidAmount > 0) {
            tile.HasTile = false;
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, i, j);
            }
            return false;
        }
        return true;
    }

    public override void RandomUpdate(int i, int j) {
        UpdateActive(i, j);
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
        return UpdateActive(i, j);
    }
}