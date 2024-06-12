using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;

namespace Aequus.Core.Utilities;

public static partial class Helper {
    /// <summary>Writes a string with no prefix or suffix to indicate length.</summary>
    public static void WriteLiteral(this BinaryWriter writer, string text, Encoding encoding = null) {
        encoding ??= Encoding.UTF8;
        Stream stream = writer.BaseStream;
        if (text != null) {
            stream.Write(encoding.GetBytes(text));
        }
    }

    public static IEnumerable<Vector2> CircularVector(int count, float baseValue = 0f) {
        float amount = 1f / count * MathHelper.TwoPi;
        do {
            yield return (amount * count + baseValue).ToRotationVector2();
        }
        while (--count > 0);
    }

    public static IEnumerable<float> Circular(int count, float baseValue = 0f) {
        float amount = 1f / count;
        do {
            yield return amount * count + baseValue;
        }
        while (--count > 0);
    }

    /// <returns>Returns 0f if <see cref="float.IsNaN(float)"/> returns true for <paramref name="value"/>.</returns>
    public static float UnNaN(this float value) {
        return float.IsNaN(value) ? 0f : value;
    }
    /// <returns><paramref name="value"/> but replaces <see cref="Vector2.X"/> or <see cref="Vector2.Y"/> with <c>0f</c> if <see cref="float.IsNaN(float)"/> is true for their values.</returns>
    public static Vector2 UnNaN(this Vector2 value) {
        return new Vector2(UnNaN(value.X), UnNaN(value.Y));
    }

    /// <summary>
    /// Subtracts <paramref name="minThreshold"/> from <paramref name="value"/>, then multiplies the result by <paramref name="scaleMultiplier"/>, finally re-adding <paramref name="minThreshold"/> back.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minThreshold"></param>
    /// <param name="scaleMultiplier"></param>
    /// <returns></returns>
    public static float MultiplyAboveMin(float value, float minThreshold, float scaleMultiplier) {
        return (value - minThreshold) * scaleMultiplier + minThreshold;
    }

    /// <summary>Converts <paramref name="value"/> to an int. And uses RNG to determine if value should be +1 higher depending on the remaining decimal number. For example, 10.5 has a 50% chance to return either 10 or 11, since the remaining decimal value gives it a 50% chance to be +1 higher.</summary>
    /// <param name="value"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public static int ToIntUsingRNGForDecimals(float value, UnifiedRandom random) {
        int integerValue = (int)value;

        value -= integerValue;

        if (value > 0f && random.NextFloat(1f) < value) {
            integerValue++;
        }

        return integerValue;
    }

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
    public static bool InOuterPercentOfWorld(float tileX, float percent) {
        if (percent <= 0f || percent >= 0.5f) {
            throw new ArgumentException("Must be a value between 0 and 0.5.", nameof(percent));
        }

        float left = Main.maxTilesX * percent;
        float right = Main.maxTilesX - left;

        return tileX < left || tileX > right;
    }

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