using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;

namespace Aequus.Core.Utilities;

public static class Helper {
    public static bool TryReadInt(string s, out int value) {
        return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out value);
    }

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

    public static int FindTarget(Vector2 position, int width = 2, int height = 2, float maxRange = 800f, object me = null, Func<int, bool> validCheck = null) {
        float num = maxRange;
        int result = -1;
        var center = position + new Vector2(width / 2f, height / 2f);
        for (int i = 0; i < 200; i++) {
            NPC nPC = Main.npc[i];
            if (nPC.CanBeChasedBy(me) && (validCheck == null || validCheck.Invoke(i))) {
                float num2 = Vector2.Distance(center, Main.npc[i].Center);
                if (num2 < num) {
                    num = num2;
                    result = i;
                }
            }
        }
        return result;
    }

    public static int FindTargetWithLineOfSight(Vector2 position, int width = 2, int height = 2, float maxRange = 800f, object me = null, Func<int, bool> validCheck = null) {
        float num = maxRange;
        int result = -1;
        var center = position + new Vector2(width / 2f, height / 2f);
        for (int i = 0; i < 200; i++) {
            NPC nPC = Main.npc[i];
            if (nPC.CanBeChasedBy(me) && (validCheck == null || validCheck.Invoke(i))) {
                float num2 = Vector2.Distance(center, Main.npc[i].Center);
                if (num2 < num && Collision.CanHit(position, width, height, nPC.position, nPC.width, nPC.height)) {
                    num = num2;
                    result = i;
                }
            }
        }
        return result;
    }

    #region Type
    public static string NamespaceFilePath(this Type t) {
        return t.Namespace.Replace('.', '/');
    }
    public static string NamespaceFilePath(this object obj) {
        return obj.GetType().NamespaceFilePath();
    }
    public static string NamespaceFilePath<T>() {
        return typeof(T).NamespaceFilePath();
    }

    public static string GetFilePath(this Type t) {
        return $"{t.NamespaceFilePath()}/{t.Name}";
    }
    public static string GetFilePath(this object obj) {
        return GetFilePath(obj.GetType());
    }
    public static string GetFilePath<T>() {
        return GetFilePath(typeof(T));
    }
    #endregion

    #region RNG
    public static KeyValuePair<TKey, TValue> NextPair<TKey, TValue>(this UnifiedRandom random, IDictionary<TKey, TValue> dictionary) {
        return random.NextFromList(dictionary.ToArray());
    }
    public static TValue NextValue<TKey, TValue>(this UnifiedRandom random, IDictionary<TKey, TValue> dictionary) {
        return random.NextFromList(dictionary.Values.ToArray());
    }
    public static TKey NextKey<TKey, TValue>(this UnifiedRandom random, IDictionary<TKey, TValue> dictionary) {
        return random.NextFromList(dictionary.Keys.ToArray());
    }

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

    #region Misc Item Stuff
    public static bool TryStackingInto(this Item[] inv, int maxSlots, Item item, out int i) {
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

    public static void RemoveItemId(this ILoot loot, int itemId) {
        var dropRules = loot.Get(includeGlobalDrops: false);
        for (int i = 0; i < dropRules.Count; i++) {
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
    public static bool FrozenTimeActive() {
        return CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled;
    }

    public static int GetTimeScale() {
        if (FrozenTimeActive()) {
            return 0;
        }

        return CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
    }

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