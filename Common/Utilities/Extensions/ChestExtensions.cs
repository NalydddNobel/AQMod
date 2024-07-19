using Aequus.Common.DataSets;
using Aequus.Common.Tiles;
using Aequus.Content.Chests;
using System;
using System.Collections.Generic;

namespace Aequus.Common.Utilities.Extensions;

public static class ChestExtensions {
    /// <returns>Whether the chest contains any items within the <see cref="ItemDataSet.ImportantItem"/> set.</returns>
    public static bool HasImportantItem(this Chest chest) {
        for (int i = 0; i < chest.item.Length; i++) {
            Item item = chest.item[i];
            if (item != null && !item.IsAir && ItemSets.ImportantItem.Contains(item.type)) {
                return true;
            }
        }

        return false;
    }

    /// <returns>Whether the chest contains <see cref="UnopenedChestItem"/>.</returns>
    public static bool IsUnopened(this Chest chest) {
        int unopenedChestItemId = ModContent.ItemType<UnopenedChestItem>();
        for (int i = 0; i < chest.item.Length; i++) {
            Item item = chest.item[i];
            if (item != null && !item.IsAir && item.type == unopenedChestItemId) {
                return true;
            }
        }

        return false;
    }

    public static Item FindEmptySlot(this Chest chest) {
        for (int i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] == null) {
                chest.item[i] = new();
            }
            if (chest.item[i].IsAir) {
                return chest.item[i];
            }
        }
        return null;
    }

    public static void InsertItem(this Chest chest, Item item, int slot) {
        if (!chest.item.IndexInRange(slot)) {
            return;
        }
        Item compareItem = chest.item[slot];

        // Try stacking the items first.
        if (ItemLoader.TryStackItems(compareItem, item, out int transfered) && item.stack <= 0) {
            return;
        }

        // Insert the item into the slot otherwise, pushing other items forward.
        chest.ForceInsertItem(item, slot);
    }

    public static void ForceInsertItem(this Chest chest, Item item, int slot) {
        int endSlot = slot;
        int length = chest.item.Length - 1;
        for (; endSlot < length && chest.item[endSlot].IsAir; ++endSlot) ;

        if (endSlot != slot) {
            for (int i = endSlot; i >= slot; i--) {
                Main.chest[i + 1] = Main.chest[i];
            }
        }

        chest.item[slot] = item;
    }

    public static Item AddItem(this Chest chest, int item, int stack = 1, int prefix = -1) {
        var emptySlot = chest.FindEmptySlot();
        if (emptySlot != null) {
            emptySlot.SetDefaults(item);
            emptySlot.stack = stack;
            if (prefix != 0) {
                emptySlot.Prefix(prefix);
            }
        }

        return emptySlot;
    }

    public static bool IsGenericUndergroundChest(Chest chest) {
        return ChestType.IsGenericUndergroundChest.Contains(new TileKey(Main.tile[chest.x, chest.y].TileType, TileHelper.GetStyle(chest.x, chest.y, coordinateFullWidthBackup: 36)));
    }

    public static Item FindFirst(this Chest chest, int itemId) {
        for (int i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] != null && !chest.item[i].IsAir && chest.item[i].type == itemId) {
                return chest.item[i];
            }
        }
        return null;
    }

    public static IEnumerable<Item> Where(this Chest chest, Predicate<Item> predicate) {
        for (int i = 0; i < Chest.maxItems; i++) {
            if (predicate(chest.item[i])) {
                yield return chest.item[i];
            }
        }
    }

    public static Item ReplaceFirst(this Chest chest, int itemId, int newItemId, int newStack = -1) {
        var item = chest.FindFirst(itemId);
        if (item == null) {
            return item;
        }

        int stack = newStack <= 0 ? item.stack : newStack;
        item.SetDefaults(newItemId);
        item.stack = stack;
        return item;
    }

    public static bool RemoveAllItemIds(this Chest chest, int itemId) {
        bool anyRemoved = false;
        for (int i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] == null) {
                chest.item[i] = new();
                continue;
            }
            if (!chest.item[i].IsAir && chest.item[i].type == itemId) {
                chest.Remove(i);
                anyRemoved = true;
                i--;
                continue;
            }
        }
        return anyRemoved;
    }

    public static bool Remove(this Chest chest, int index) {
        chest.item[index] = new();
        for (int i = index; i < Chest.maxItems - 1; i++) {
            chest.item[i] = chest.item[i + 1];
        }
        return true;
    }

    public static bool IsSynced(this Chest chest) {
        if (chest.item == null) {
            return false;
        }

        for (int i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] == null) {
                return false;
            }
        }
        return true;
    }
}
