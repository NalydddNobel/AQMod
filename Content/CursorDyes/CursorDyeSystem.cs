using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Aequus.Content.CursorDyes
{
    public class CursorDyeSystem : ModSystem
    {
        public static Color cursorColorOld;

        private static Dictionary<int, ICursorDye> itemIDToCursor;
        private static Dictionary<int, int> cursorToItemID;
        private static List<ICursorDye> cursorDyes;
        public static HashSet<int> IsCursorAcc { get; private set; }

        public override void Load()
        {
            IsCursorAcc = new HashSet<int>() { ItemID.RainbowCursor, };
            cursorDyes = new List<ICursorDye>();
            itemIDToCursor = new Dictionary<int, ICursorDye>();
            cursorToItemID = new Dictionary<int, int>();
            On.Terraria.Main.DrawThickCursor += Main_DrawThickCursor;
            On.Terraria.Main.DrawCursor += Main_DrawCursor;
            On.Terraria.Main.CursorColor += Main_CursorColor;
            On.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;
        }

        private static void Main_DrawInterface_36_Cursor(On.Terraria.Main.orig_DrawInterface_36_Cursor orig)
        {
            if (CanDrawCustomCursor() && !PlayerInput.UsingGamepad && LocalCursor(out var cursor))
            {
                var bonus = Vector2.Zero;
                bool smart = false;
                if (cursor.PreDrawCursor(ref bonus, ref smart))
                {
                    orig();
                }
                cursor.PostDrawCursor(bonus, smart);
                return;
            }
            orig();
        }

        private static void Main_CursorColor(On.Terraria.Main.orig_CursorColor orig)
        {
            if (CanDrawCustomCursor() && LocalCursor(out var cursor))
            {
                var c = cursor.GetCursorColor();
                if (c != null)
                {
                    cursorColorOld = Main.mouseColor;

                    Main.mouseColor = c.Value;
                    orig();
                    Main.mouseColor = cursorColorOld;
                    return;
                }
            }
            orig();
        }

        private static Vector2 Main_DrawThickCursor(On.Terraria.Main.orig_DrawThickCursor orig, bool smart)
        {
            if (CanDrawCustomCursor() && !PlayerInput.UsingGamepad && LocalCursor(out var cursor))
            {
                var bonus = Vector2.Zero;
                if (cursor.DrawThickCursor(ref bonus, ref smart))
                {
                    bonus += orig(smart);
                }
                return bonus;
            }
            return orig(smart);
        }
        private static void Main_DrawCursor(On.Terraria.Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
        {
            if (CanDrawCustomCursor() && !PlayerInput.UsingGamepad && LocalCursor(out var cursor))
            {
                if (cursor.PreDrawCursor(ref bonus, ref smart))
                {
                    orig(bonus, smart);
                }
                cursor.PostDrawCursor(bonus, smart);
                return;
            }
            orig(bonus, smart);
        }

        public static int Register(int item, ICursorDye data)
        {
            int type = Register(data);
            itemIDToCursor.Add(item, data);
            cursorToItemID.Add(type, item);
            return type;
        }
        public static int Register(ICursorDye data)
        {
            data.Type = cursorDyes.Count;
            cursorDyes.Add(data);
            return data.Type;
        }

        public static int ItemIDToCursorID(int itemID)
        {
            if (itemIDToCursor.TryGetValue(itemID, out var cursor))
                return cursor.Type;
            return -1;
        }
        public static ICursorDye ItemIDToCursor(int itemID)
        {
            return itemIDToCursor[itemID];
        }
        public static T ItemIDToCursor<T>(int itemID) where T : class, ICursorDye
        {
            return (T)ItemIDToCursor(itemID);
        }

        public static int CursorToItemID(int cursor)
        {
            return cursorToItemID[cursor];
        }
        public static int CursorToItemID(ICursorDye cursor)
        {
            return CursorToItemID(cursor.Type);
        }

        public static bool LocalCursor(out ICursorDye cursor)
        {
            if (Main.LocalPlayer.Aequus().CursorDye > -1)
            {
                cursor = cursorDyes[Main.LocalPlayer.Aequus().CursorDye];
                return true;
            }
            cursor = null;
            return false;
        }

        internal static bool CanDrawCustomCursor()
        {
            return !Main.gameMenu && Main.myPlayer >= 0 && Main.LocalPlayer != null && Main.player[Main.myPlayer].active;
        }
    }
}