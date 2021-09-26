using AQMod.Content.CursorDyes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;

namespace AQMod.Common.CursorDyes
{
    public class CursorDyeManager
    {
        private static int _lastDyeID;

        private static List<CursorDye> _dyes;

        public static class ID
        {
            public const ushort None = 0;
            public const ushort Health = 1;
            public const ushort Mana = 2;
            public const ushort Sword = 3;
        }

        public static CursorDye CursorDyeFromID(int type)
        {
            return _dyes[type - 1];
        }

        public static int CursorDyeIDFromKey(string mod, string name)
        {
            return _dyes.FindIndex((c) => c.PredicateMatch(mod, name)) + 1;
        }

        public static CursorDye CursorDyeFromKey(string mod, string name)
        {
            return _dyes.Find((c) => c.PredicateMatch(mod, name));
        }

        public static int CursorDyeCount => _lastDyeID;

        public static int LoadDye(CursorDye dye)
        {
            if (AQMod.Loading)
            {
                if (_lastDyeID == 0 || CursorDyeIDFromKey(dye.Mod, dye.Name) == 0)
                {
                    _lastDyeID++;
                    _dyes.Add(dye);
                }
            }
            return -1;
        }

        internal static void Setup()
        {
            On.Terraria.Main.CursorColor += Main_CursorColor;
            On.Terraria.Main.DrawCursor += Main_DrawCursor;
            On.Terraria.Main.DrawThickCursor += Main_DrawThickCursor;
            On.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;
        }

        private static Vector2 Main_DrawThickCursor(On.Terraria.Main.orig_DrawThickCursor orig, bool smart)
        {
            if (CanApplyCursorDye())
            {
                var type = Main.LocalPlayer.GetModPlayer<AQVisualsPlayer>().CursorDyeID;
                if (type != ID.None)
                {
                    var value = CursorDyeFromID(type).DrawThickCursor(smart);
                    if (value != null)
                        return value.Value;
                }
            }
            return orig(smart);
        }

        private static void Main_DrawCursor(On.Terraria.Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
        {
            if (CanApplyCursorDye() && !PlayerInput.UsingGamepad)
            {
                var player = Main.LocalPlayer;
                var drawingPlayer = player.GetModPlayer<AQVisualsPlayer>();
                if (drawingPlayer.CursorDyeID != ID.None)
                {
                    var cursorDye = CursorDyeFromID(drawingPlayer.CursorDyeID);
                    if (!cursorDye.PreDrawCursor(player, drawingPlayer, bonus, smart))
                        orig(bonus, smart);
                    cursorDye.PostDrawCursor(player, drawingPlayer, bonus, smart);
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

        private static void Main_DrawInterface_36_Cursor(On.Terraria.Main.orig_DrawInterface_36_Cursor orig)
        {
            if (CanApplyCursorDye())
            {
                var player = Main.LocalPlayer;
                var drawingPlayer = player.GetModPlayer<AQVisualsPlayer>();
                if (drawingPlayer.CursorDyeID != ID.None)
                {
                    var cursorDye = CursorDyeFromID(drawingPlayer.CursorDyeID);
                    if (!cursorDye.PreDrawCursorOverrides(player, drawingPlayer))
                        orig();
                    cursorDye.PostDrawCursorOverrides(player, drawingPlayer);
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

        private static void Main_CursorColor(On.Terraria.Main.orig_CursorColor orig)
        {
            if (CursorDyeManager.CanApplyCursorDye() && CursorDyeManager.OverrideColor)
            {
                CursorDyeManager.ApplyNewColor();
                orig();
                CursorDyeManager.ResetColor();
            }
            else
            {
                orig();
            }
        }

        internal static void InitializeDyes(AQMod aQMod)
        {
            _dyes = new List<CursorDye>();
            LoadDye(new CursorDyeHealth(aQMod, "Health"));
            LoadDye(new CursorDyeMana(aQMod, "Mana"));
            LoadDye(new CursorDyeSword(aQMod, "Sword"));
        }

        internal static void Unload()
        {
            _dyes = null;
            _lastDyeID = 0;
        }

        private static Color _oldCursorColor;
        private static Color _newCursorColor;
        public static bool OverrideColor { get; private set; }

        internal static bool CanApplyCursorDye()
        {
            return !Main.gameMenu && !AQMod.Loading && Main.myPlayer != -1 && Main.LocalPlayer.active;
        }

        internal static void UpdateColor()
        {
            OverrideColor = false;
            var player = Main.LocalPlayer;
            var drawingPlayer = player.GetModPlayer<AQVisualsPlayer>();
            if (drawingPlayer.CursorDyeID != ID.None)
            {
                var cursorDye = CursorDyeFromID(drawingPlayer.CursorDyeID);
                OverrideColor = cursorDye.ApplyColor(player, drawingPlayer, out _newCursorColor);
            }
        }

        internal static void ApplyNewColor()
        {
            _oldCursorColor = Main.mouseColor;
            Main.mouseColor = _newCursorColor;
        }

        internal static void ResetColor()
        {
            Main.mouseColor = _oldCursorColor;
            _newCursorColor = Main.mouseColor;
        }
    }
}