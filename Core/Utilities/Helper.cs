using Aequus.Common.Tiles;
using System;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;

namespace Aequus.Core.Utilities;

public static class Helper {
    public static Point WorldClamp(this Point value, Int32 fluff = 0) {
        return new(Math.Clamp(value.X, fluff, Main.maxTilesX - fluff), Math.Clamp(value.Y, fluff, Main.maxTilesX - fluff));
    }

    public static Single Oscillate(Single time, Single magnitude) {
        return Oscillate(time, 0f, magnitude);
    }

    public static Single Oscillate(Single time, Single minimum, Single maximum) {
        return (Single)(minimum + (Math.Sin(time) + 1f) / 2f * (maximum - minimum));
    }

    public static Rectangle Frame(this Rectangle rectangle, Int32 frameX, Int32 frameY, Int32 sizeOffsetX = 0, Int32 sizeOffsetY = 0) {
        return new Rectangle(rectangle.X + (rectangle.Width - sizeOffsetX) * frameX, rectangle.Y + (rectangle.Width - sizeOffsetY) * frameY, rectangle.Width, rectangle.Height);
    }

    public static Boolean IsFalling(Vector2 velocity, Single gravDir) {
        return Math.Sign(velocity.Y) == Math.Sign(gravDir);
    }

    public static Int32 FindTarget(Vector2 position, Int32 width = 2, Int32 height = 2, Single maxRange = 800f, Object me = null, Func<Int32, Boolean> validCheck = null) {
        Single num = maxRange;
        Int32 result = -1;
        var center = position + new Vector2(width / 2f, height / 2f);
        for (Int32 i = 0; i < 200; i++) {
            NPC nPC = Main.npc[i];
            if (nPC.CanBeChasedBy(me) && (validCheck == null || validCheck.Invoke(i))) {
                Single num2 = Vector2.Distance(center, Main.npc[i].Center);
                if (num2 < num) {
                    num = num2;
                    result = i;
                }
            }
        }
        return result;
    }

    public static Int32 FindTargetWithLineOfSight(Vector2 position, Int32 width = 2, Int32 height = 2, Single maxRange = 800f, Object me = null, Func<Int32, Boolean> validCheck = null) {
        Single num = maxRange;
        Int32 result = -1;
        var center = position + new Vector2(width / 2f, height / 2f);
        for (Int32 i = 0; i < 200; i++) {
            NPC nPC = Main.npc[i];
            if (nPC.CanBeChasedBy(me) && (validCheck == null || validCheck.Invoke(i))) {
                Single num2 = Vector2.Distance(center, Main.npc[i].Center);
                if (num2 < num && Collision.CanHit(position, width, height, nPC.position, nPC.width, nPC.height)) {
                    num = num2;
                    result = i;
                }
            }
        }
        return result;
    }

    #region Type
    public static String NamespaceFilePath(this Type t) {
        return t.Namespace.Replace('.', '/');
    }
    public static String NamespaceFilePath(this Object obj) {
        return obj.GetType().NamespaceFilePath();
    }
    public static String NamespaceFilePath<T>() {
        return typeof(T).NamespaceFilePath();
    }

    public static String GetFilePath(this Type t) {
        return $"{t.NamespaceFilePath()}/{t.Name}";
    }
    public static String GetFilePath(this Object obj) {
        return GetFilePath(obj.GetType());
    }
    public static String GetFilePath<T>() {
        return GetFilePath(typeof(T));
    }
    #endregion

    #region RNG
    public static Single NextFloat(this ref FastRandom random, Single min, Single max) {
        return min + random.NextFloat() * (max - min);
    }
    public static Single NextFloat(this ref FastRandom random, Single max) {
        return random.NextFloat() * max;
    }

    public static UInt64 TileSeed(Int32 i, Int32 j) {
        UInt64 x = (UInt64)i;
        UInt64 y = (UInt64)j;
        return x * x + y * y * x + x;
    }
    public static UInt64 TileSeed(Point point) {
        return TileSeed(point.X, point.Y);
    }

    /// <summary>
    /// Gets a consistent <see cref="FastRandom"/> for these tile coordinates.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public static FastRandom RandomTileCoordinates(Int32 i, Int32 j) {
        return new(TileSeed(i, j));
    }
    #endregion

    #region Chests & Loot
    public static Boolean IsGenericUndergroundChest(Chest chest) {
        return ChestType.GenericUndergroundChest.Contains(new TileKey(Main.tile[chest.x, chest.y].TileType, TileHelper.GetStyle(chest.x, chest.y, coordinateFullWidthBackup: 36)));
    }

    public static Item FindFirst(this Chest chest, Int32 itemId) {
        for (Int32 i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] != null && !chest.item[i].IsAir && chest.item[i].type == itemId) {
                return chest.item[i];
            }
        }
        return null;
    }

    public static Item ReplaceFirst(this Chest chest, Int32 itemId, Int32 newItemId, Int32 newStack = -1) {
        var item = chest.FindFirst(itemId);
        if (item == null) {
            return item;
        }

        Int32 stack = newStack <= 0 ? item.stack : newStack;
        item.SetDefaults(newItemId);
        item.stack = stack;
        return item;
    }

    public static Boolean RemoveAllItemIds(this Chest chest, Int32 itemId) {
        Boolean anyRemoved = false;
        for (Int32 i = 0; i < Chest.maxItems; i++) {
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

    public static Boolean Remove(this Chest chest, Int32 index) {
        chest.item[index] = new();
        for (Int32 i = index; i < Chest.maxItems - 1; i++) {
            chest.item[i] = chest.item[i + 1];
        }
        return true;
    }

    public static Boolean IsSynced(this Chest chest) {
        if (chest.item == null) {
            return false;
        }

        for (Int32 i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] == null) {
                return false;
            }
        }
        return true;
    }

    public static Boolean TryStackingInto(this Item[] inv, Int32 maxSlots, Item item, out Int32 i) {
        i = -1;
        while (item.stack > 0) {
            i = inv.FindSuitableSlot(maxSlots, item);
            if (i == -1) {
                return false;
            }

            if (inv[i].IsAir) {
                inv[i] = item.Clone();
                return true;
            }
            Int32 stack = inv[i].stack + item.stack;
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

    public static Int32 FindSuitableSlot(this Item[] inv, Int32 maxSlots, Item item) {
        if (item.stack != item.maxStack) {
            for (Int32 i = 0; i < Chest.maxItems; i++) {
                if (inv[i].type == item.type && inv[i].stack < inv[i].maxStack && inv[i].prefix == item.prefix && ItemLoader.CanStack(item, inv[i]))
                    return i;
            }
        }
        for (Int32 i = 0; i < Chest.maxItems; i++) {
            if (inv[i].IsAir)
                return i;
        }
        return -1;
    }

    public static Item FindEmptySlot(this Chest chest) {
        for (Int32 i = 0; i < Chest.maxItems; i++) {
            if (chest.item[i] == null) {
                chest.item[i] = new();
            }
            if (chest.item[i].IsAir) {
                return chest.item[i];
            }
        }
        return null;
    }

    public static Item AddItem(this Chest chest, Int32 item, Int32 stack = 1, Int32 prefix = -1) {
        Item emptySlot = chest.FindEmptySlot();

        if (emptySlot != null) {
            emptySlot.SetDefaults(item);
            emptySlot.stack = stack;
            if (prefix != 0) {
                emptySlot.Prefix(prefix);
            }
        }

        return emptySlot;
    }

    public static void RemoveItemId(this ILoot loot, Int32 itemId) {
        var dropRules = loot.Get(includeGlobalDrops: false);
        for (Int32 i = 0; i < dropRules.Count; i++) {
            IItemDropRule l = dropRules[i];
            if (l is CommonDrop commonDrop && commonDrop.itemId == itemId) {
                loot.Remove(l);
            }
            else if (l is OneFromOptionsDropRule oneFromOptions) {
                ExtendArray.Remove(ref oneFromOptions.dropIds, itemId);
            }
            else if (l is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsNotScaledWithLuck) {
                ExtendArray.Remove(ref oneFromOptionsNotScaledWithLuck.dropIds, itemId);
            }
        }
    }
    #endregion

    #region World
    public static Boolean FrozenTimeActive() {
        return CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled;
    }

    public static Int32 GetTimeScale() {
        if (FrozenTimeActive()) {
            return 0;
        }

        return CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
    }

    public static Double ZoneSkyHeightY => Main.worldSurface * 0.35;

    public static Boolean ZoneSkyHeight(Entity entity) {
        return ZoneSkyHeight(entity.position.Y);
    }
    public static Boolean ZoneSkyHeight(Single worldY) {
        return ZoneSkyHeight((Int32)worldY / 16);
    }
    public static Boolean ZoneSkyHeight(Int32 tileY) {
        return tileY < ZoneSkyHeightY;
    }
    #endregion
}