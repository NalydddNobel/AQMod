using Aequus.Common.Tiles;
using Aequus.Core.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus;

public static class Helper {
    public static Point WorldClamp(this Point value, int fluff = 0) {
        return new(Math.Clamp(value.X, fluff, Main.maxTilesX - fluff), Math.Clamp(value.Y, fluff, Main.maxTilesX - fluff));
    }

    public static float Oscillate(float time, float magnitude) {
        return Oscillate(time, 0f, magnitude);
    }

    public static float Oscillate(float time, float minimum, float maximum) {
        return (float)(minimum + (Math.Sin(time) + 1f) / 2f * (maximum - minimum));
    }

    public static Rectangle Frame(this Rectangle rectangle, int frameX, int frameY, int sizeOffsetX = 0, int sizeOffsetY = 0) {
        return new Rectangle(rectangle.X + (rectangle.Width - sizeOffsetX) * frameX, rectangle.Y + (rectangle.Width - sizeOffsetY) * frameY, rectangle.Width, rectangle.Height);
    }

    public static bool IsFalling(Vector2 velocity, float gravDir) {
        return Math.Sign(velocity.Y) == Math.Sign(gravDir);
    }

    #region Type
    public static string NamespaceFilePath(this Type t) {
        return t.Namespace.Replace('.', '/');
    }
    public static string NamespaceFilePath(this object obj) {
        return NamespaceFilePath(obj.GetType());
    }
    public static string NamespaceFilePath<T>() {
        return NamespaceFilePath(typeof(T));
    }

    public static string GetFilePath(this object obj) {
        return GetFilePath(obj.GetType());
    }
    public static string GetFilePath<T>() {
        return GetFilePath(typeof(T));
    }
    public static string GetFilePath(Type t) {
        return $"{NamespaceFilePath(t)}/{t.Name}";
    }
    #endregion

    #region RNG
    public static float NextFloat(this ref FastRandom random, float min, float max) {
        return min + random.NextFloat() * (max - min);
    }
    public static float NextFloat(this ref FastRandom random, float max) {
        return random.NextFloat() * max;
    }

    public static ulong TileSeed(int i, int j) {
        ulong x = (ulong)i;
        ulong y = (ulong)j;
        return x * x + y * y * x + x;
    }
    public static ulong TileSeed(Point point) {
        return TileSeed(point.X, point.Y);
    }

    /// <summary>
    /// Gets a consistent <see cref="FastRandom"/> for these tile coordinates.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public static FastRandom RandomTileCoordinates(int i, int j) {
        return new(TileSeed(i, j));
    }
    #endregion

    #region Chests & Loot
    public static bool IsGenericUndergroundChest(Chest chest) {
        return ChestType.GenericUndergroundChest.Contains(new TileKey(Main.tile[chest.x, chest.y].TileType, TileHelper.GetStyle(chest.x, chest.y, coordinateFullWidthBackup: 36)));
    }

    public static Item FindFirst(this Chest chest, int itemId) {
        for (int i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] != null && !chest.item[i].IsAir && chest.item[i].type == itemId) {
                return chest.item[i];
            }
        }
        return null;
    }

    public static Item ReplaceFirst(this Chest chest, int itemId, int newItemId, int newStack = -1) {
        var item = FindFirst(chest, itemId);
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
                Remove(chest, i);
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

    public static bool TryStackingInto(this Item[] inv, int maxSlots, Item item, out int i) {
        i = -1;
        while (item.stack > 0) {
            i = FindSuitableSlot(inv, maxSlots, item);
            if (i == -1) {
                return false;
            }

            if (inv[i].IsAir) {
                inv[i] = item.Clone();
                return true;
            }
            int stack = inv[i].stack + item.stack;
            if (stack > inv[i].maxStack) {
                item.stack = stack - inv[i].maxStack;
                inv[i].stack = inv[i].maxStack;
                continue;
            }
            inv[i].stack = stack;
            return true;
        }
        return false;
    }

    public static int FindSuitableSlot(this Item[] inv, int maxSlots, Item item) {
        if (item.stack != item.maxStack) {
            for (int i = 0; i < Chest.maxItems; i++) {
                if (inv[i].type == item.type && inv[i].stack < inv[i].maxStack && inv[i].prefix == item.prefix && ItemLoader.CanStack(item, inv[i]))
                    return i;
            }
        }
        for (int i = 0; i < Chest.maxItems; i++) {
            if (inv[i].IsAir)
                return i;
        }
        return -1;
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

    public static Item AddItem(this Chest chest, int item, int stack = 1, int prefix = 0) {
        var emptySlot = chest.FindEmptySlot();
        if (emptySlot != null) {
            emptySlot.SetDefaults(item);
            emptySlot.stack = stack;
            if (prefix > 0)
                emptySlot.Prefix(prefix);
        }
        return emptySlot;
    }

    public static void RemoveItemId(this ILoot loot, int itemId) {
        var dropRules = loot.Get(includeGlobalDrops: false);
        for (int i = 0; i < dropRules.Count; i++) {
            IItemDropRule l = dropRules[i];
            if (l is CommonDrop commonDrop && commonDrop.itemId == itemId) {
                loot.Remove(l);
            }
            else if (l is OneFromOptionsDropRule oneFromOptions) {
                EnumerableHelper.Remove(ref oneFromOptions.dropIds, itemId);
            }
            else if (l is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsNotScaledWithLuck) {
                EnumerableHelper.Remove(ref oneFromOptionsNotScaledWithLuck.dropIds, itemId);
            }
        }
    }
    #endregion

    #region World
    public static double ZoneSkyHeightY => Main.worldSurface * 0.35;

    public static bool ZoneSkyHeight(Entity entity) {
        return ZoneSkyHeight(entity.position.Y);
    }
    public static bool ZoneSkyHeight(float worldY) {
        return ZoneSkyHeight((int)worldY / 16);
    }
    public static bool ZoneSkyHeight(int tileY) {
        return tileY < ZoneSkyHeightY;
    }
    #endregion
}