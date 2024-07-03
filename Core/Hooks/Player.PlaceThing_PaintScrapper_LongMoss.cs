namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for custom mosses to be broken by the Paint Scraper.</summary>
    private static void On_Player_PlaceThing_PaintScrapper_LongMoss(On_Player.orig_PlaceThing_PaintScrapper_LongMoss orig, Player player, int x, int y) {
        //if (Main.tile[x, y].TileType == ModContent.TileType<RadonMoss>()) {
        //    player.cursorItemIconEnabled = true;
        //    if (!player.ItemTimeIsZero || player.itemAnimation <= 0 || !player.controlUseItem) {
        //        return;
        //    }
        //    WorldGen.KillTile(x, y);
        //    if (Main.tile[x, y].HasTile) {
        //        return;
        //    }
        //    player.ApplyItemTime(player.HeldItem);
        //    if (Main.netMode == NetmodeID.MultiplayerClient) {
        //        NetMessage.SendData(MessageID.TileManipulation, number: x, number2: y);
        //    }
        //    if (Main.rand.NextBool(9)) {
        //        int item = Item.NewItem(player.GetSource_ItemUse(player.HeldItem), x * 16, y * 16, 16, 16, RadonMoss.Item.Type);
        //        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
        //    }
        //    return;
        //}

        orig(player, x, y);
    }
}
