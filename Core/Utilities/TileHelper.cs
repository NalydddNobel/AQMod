using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Aequus.Core.Utilities;

public static class TileHelper {
    public static void CutTilesRectangle(Rectangle box, TileCuttingContext context, bool[] tileCutIgnore) {
        int left = Math.Max((int)(box.X / 16f), 1);
        int right = Math.Min((int)(left + box.Width / 16f) + 1, Main.maxTilesX);
        int top = Math.Max((int)(box.Y / 16f), 0);
        int bottom = Math.Min((int)(top + box.Height / 16f) + 1, Main.maxTilesY);
        for (int i = left; i < right; i++) {
            for (int j = top; j < bottom; j++) {
                if (Main.tile[i, j] != null && Main.tileCut[Main.tile[i, j].TileType] && !tileCutIgnore[Main.tile[i, j].TileType] && WorldGen.CanCutTile(i, j, context)) {
                    WorldGen.KillTile(i, j);
                    if (Main.netMode != NetmodeID.SinglePlayer) {
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                    }
                }
            }
        }
    }

    public static void CutTilesRectangle(Rectangle box, TileCuttingContext context, Player player, bool fromTrap = false) {
        CutTilesRectangle(box, context, player.GetTileCutIgnorance(allowRegrowth: false, fromTrap));
    }
}
