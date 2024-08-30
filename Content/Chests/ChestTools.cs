using System;
using System.Collections.Generic;

namespace Aequus.Content.Chests;

public class ChestTools {
    static void NetUpdateInner(int chestID, int tileID, int x, int y) {
        int shorthand = 1;
        if (tileID == TileID.Containers2) {
            shorthand = 5;
        }
        if (tileID >= TileID.Count) {
            shorthand = 101;
        }

        NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, shorthand, x, y, 0f, chestID, Main.tile[x, y].TileType);
        NetMessage.SendTileSquare(-1, x, y, 3);
    }

    public static void NetUpdate(int chestID) {
        NetUpdateInner(chestID, Main.tile[Main.chest[chestID].x, Main.chest[chestID].y].TileType, Main.chest[chestID].x, Main.chest[chestID].y);
    }

    public static IEnumerable<(Chest Chest, int Index)> ForEachUnopened() {
        for (int i = 0; i < Main.maxChests; i++) {
            Chest c = Main.chest[i];
            // Check if this chest is safe to iterate over.
            // We only peek into the first item slot if it's null, this does not guarantee other slots are not null.
            if (c != null && c.item != null && c.item[0] != null && UnopenedChestTracker.IsUnopened(c)) {
                yield return (c, i);
            }
        }
    }

    public static IEnumerable<Chest> ForEach() {
        for (int i = 0; i < Main.maxChests; i++) {
            Chest c = Main.chest[i];
            // Check if this chest is safe to iterate over.
            // We only peek into the first item slot if it's null, this does not guarantee other slots are not null.
            if (c != null && c.item != null && c.item[0] != null) {
                yield return c;
            }
        }
    }

    public static void MoveItemsToLowestUnoccupiedSlot(Chest chest) {
        var l = new List<Item>();
        for (int i = 0; i < Chest.maxItems - 1; i++) {
            if (chest.item[i] != null && !chest.item[i].IsAir) {
                l.Add(chest.item[i]);
            }
        }
        for (int i = 0; i < l.Count; i++) {
            chest.item[i] = l[i].Clone();
        }
        for (int i = l.Count; i < Chest.maxItems - 1; i++) {
            if (chest.item[i] == null) {
                chest.item[i] = new Item();
            }
            else {
                chest.item[i].TurnToAir();
            }
        }
    }

    public static void StackDuplicateContents(Chest chest) {
        for (int i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] != null && chest.item[i].stack < chest.item[i].maxStack) {
                StackDuplicatesIntoSlot(chest, i);
            }
        }
    }

    public static void StackDuplicatesIntoSlot(Chest chest, int i) {
        for (int j = Chest.maxItems - 1; j > i; j--) {
            if (chest.item[j] != null && chest.item[i].type == chest.item[j].type && ItemLoader.CanStack(chest.item[i], chest.item[j])) {
                int diff = Math.Min(chest.item[i].stack + chest.item[j].stack, chest.item[i].maxStack) - chest.item[i].stack;
                chest.item[i].stack += diff;
                chest.item[j].stack -= diff;
                if (chest.item[j].stack <= 0) {
                    chest.item[j].TurnToAir();
                }
                if (chest.item[i].stack >= chest.item[i].maxStack) {
                    return;
                }
            }
        }
    }

    public static void Convert(int x, int y, ushort type, int style, bool quiet = false) {
        x -= Main.tile[x, y].TileFrameX % 36 / 18;
        y -= Main.tile[x, y].TileFrameY % 36 / 18;

        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                Tile t = Framing.GetTileSafely(x + i, y + j);
                t.HasTile = true;
                t.TileType = type;
                t.TileFrameX = (short)((i + style * 2) * 18);
                t.TileFrameY = (short)(j * 18);
            }
        }

        if (!quiet && Main.netMode != NetmodeID.SinglePlayer) {
            NetMessage.SendTileSquare(-1, x, y, 2, 2);
        }
    }

    public static bool TryGet(int id, out Chest? c) {
        if (!Main.chest.IndexInRange(id)) {
            c = null;
            return false;
        }

        c = Main.chest[id];
        return c != null;
    }
}
