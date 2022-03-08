using AQMod.Common.ID;
using AQMod.Content.CursorDyes;
using AQMod.Content.CursorDyes.Components;
using AQMod.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;

namespace AQMod.Items
{
    public sealed class CursorDyeManager
    {
        public static class Hooks
        {
            internal static Color OldCursorColor;
            internal static Color NewCursorColor;

            internal static bool ShouldApplyCustomCursor()
            {
                return !AQMod.Loading && !AQMod.IsUnloading && !Main.gameMenu && Main.myPlayer >= 0 && Main.player[Main.myPlayer] != null && Main.player[Main.myPlayer].active;
            }

            internal static Vector2 Main_DrawThickCursor(On.Terraria.Main.orig_DrawThickCursor orig, bool smart)
            {
                if (ShouldApplyCustomCursor())
                {
                    byte id = PlayerCursorDyes.LocalCursorDye;
                    if (id != CursorDyeID.None)
                    {
                        var data = GetDataFromCursorDyeID(id);
                        if (data.CursorDyeThickCursor != null)
                            return data.CursorDyeThickCursor.Value;
                    }
                }
                return orig(smart);
            }

            internal static void Main_DrawCursor(On.Terraria.Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
            {
                if (ShouldApplyCustomCursor() && !PlayerInput.UsingGamepad)
                {
                    byte id = PlayerCursorDyes.LocalCursorDye;
                    if (id != CursorDyeID.None)
                    {
                        var data = GetDataFromCursorDyeID(id);
                        if (data.PreRender(cursorOverride: false, smart))
                        {
                            orig(bonus, smart);
                        }
                        data.PostRender(cursorOverride: false, smart);
                    }
                    else
                    {
                        orig(bonus, smart);
                    }
                }
                else
                {
                    orig(bonus, smart);
                }
            }

            internal static void Main_DrawInterface_36_Cursor(On.Terraria.Main.orig_DrawInterface_36_Cursor orig)
            {
                if (ShouldApplyCustomCursor() && !PlayerInput.UsingGamepad)
                {
                    byte id = PlayerCursorDyes.LocalCursorDye;
                    if (id != CursorDyeID.None)
                    {
                        var data = GetDataFromCursorDyeID(id);
                        if (data.PreRender(cursorOverride: true, Main.SmartCursorEnabled))
                        {
                            orig();
                        }
                        data.PostRender(cursorOverride: true, Main.SmartCursorShowing);
                    }
                    else
                    {
                        orig();
                    }
                }
                else
                {
                    orig();
                }
            }

            internal static void Main_CursorColor(On.Terraria.Main.orig_CursorColor orig)
            {
                if (ShouldApplyCustomCursor() && OverridingColor)
                {
                    OldCursorColor = Main.mouseColor;
                    Main.mouseColor = NewCursorColor;
                    orig();
                    Main.mouseColor = OldCursorColor;
                    NewCursorColor = Main.mouseColor;
                }
                else
                {
                    orig();
                }
            }
        }

        private static CursorDyeData[] _data;
        public static CursorDyeData GetDataFromCursorDyeID(byte cursorDye)
        {
            return _data[cursorDye - 1];
        }
        public static bool OverridingColor { get; set; }

        internal static void Load()
        {
            _data = new CursorDyeData[CursorDyeID.NormalCount];
            _data[CursorDyeID.Health - 1] = new CursorDyeData(null, new ICursorDyeComponent[] { new CursorDyeColorChangeComponent(() => Color.Lerp(new Color(20, 1, 1, 255), new Color(255, 40, 40, 255), Main.player[Main.myPlayer].statLife / (float)Main.player[Main.myPlayer].statLifeMax2)) });
            _data[CursorDyeID.Mana - 1] = new CursorDyeData(null, new ICursorDyeComponent[] { new CursorDyeColorChangeComponent(() => Color.Lerp(new Color(255, 255, 255, 255), new Color(40, 40, 255, 255), Main.player[Main.myPlayer].statMana / (float)Main.player[Main.myPlayer].statManaMax2)) });
            _data[CursorDyeID.Sword - 1] = new CursorDyeData(new CursorDyeTextureChangeComponent("AQMod/Items/Misc/Cursor/Sword/SwordCursor"));
            _data[CursorDyeID.Demon - 1] = new CursorDyeData(new CursorDyeTextureChangeComponent("AQMod/Items/Misc/Cursor/Demonic/DemonCursor", () => Main.cursorOverride != 6));
            _data[CursorDyeID.WhackAZombie - 1] = new CursorDyeData(new WhackAZombieComponent());
        }

        internal static void Unload()
        {
            _data = null;
        }

        public static void Update()
        {
            OverridingColor = false;
            byte cursorDye = PlayerCursorDyes.LocalCursorDye;
            if (cursorDye != CursorDyeID.None)
            {
                GetDataFromCursorDyeID(cursorDye).UpdateComponents();
            }
        }

        internal static string InternalGetOverrideName(int cursorOverride)
        {
            switch (cursorOverride)
            {
                case CursorOverrideID.None:
                    return "";
                case CursorOverrideID.Smart:
                    return "_smart";
                case CursorOverrideID.Search:
                    return "_search";
                case CursorOverrideID.Favorite:
                    return "_favorite";
                case CursorOverrideID.Trash:
                    return "_trash";
                case CursorOverrideID.Into:
                    return "_into";
                case CursorOverrideID.IntoChest:
                    return "_intochest";
                case CursorOverrideID.FromChest:
                    return "_fromchest";
                case CursorOverrideID.Sell:
                    return "_sell";
            }
            return "_";
        }
    }
}