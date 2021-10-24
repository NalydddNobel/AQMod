using AQMod.Assets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;

namespace AQMod.Content.CursorDyes
{
    public class CursorDyeLoader : ContentLoader<CursorDye>
    {
        public static class ID
        {
            public const int None = -1;
            public const int Health = 0;
            public const int Mana = 1;
            public const int Sword = 2;
            public const int Demon = 3;
        }

        public override void Setup(bool setupStatics = false)
        {
            base.Setup(setupStatics);
            On.Terraria.Main.CursorColor += Main_CursorColor;
            On.Terraria.Main.DrawCursor += Main_DrawCursor;
            On.Terraria.Main.DrawThickCursor += Main_DrawThickCursor;
            On.Terraria.Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;

            var mod = AQMod.Instance;
            AddContent(new CursorDyeHealth(mod, "Health"));
            AddContent(new CursorDyeMana(mod, "Mana"));
            AddContent(new CursorDyeSword(mod, "Sword"));
            AddContent(new CursorDyeDemon(mod, "Demon"));
        }

        // saves the cursor color and stuff
        private static Color _oldCursorColor;
        private static Color _newCursorColor;
        public static bool OverrideColor { get; private set; }

        internal static bool CanApplyCursorDye()
        {
            return !AQMod.Loading && !Main.gameMenu && Main.myPlayer >= 0 && Main.LocalPlayer.active;
        }

        internal static void UpdateColor()
        {
            OverrideColor = false;
            var player = Main.LocalPlayer;
            var drawingPlayer = player.GetModPlayer<AQPlayer>();
            if (drawingPlayer.CursorDyeID != ID.None)
            {
                var cursorDye = AQMod.CursorDyes.GetContent(drawingPlayer.CursorDyeID);
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

        // ON edits
        private static Vector2 Main_DrawThickCursor(On.Terraria.Main.orig_DrawThickCursor orig, bool smart)
        {
            if (CanApplyCursorDye())
            {
                var type = Main.LocalPlayer.GetModPlayer<AQPlayer>().CursorDyeID;
                if (type != ID.None)
                {
                    var value = AQMod.CursorDyes.GetContent(type).DrawThickCursor(smart);
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
                var drawingPlayer = player.GetModPlayer<AQPlayer>();
                if (drawingPlayer.CursorDyeID != ID.None)
                {
                    var cursorDye = AQMod.CursorDyes.GetContent(drawingPlayer.CursorDyeID);
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
                var drawingPlayer = player.GetModPlayer<AQPlayer>();
                if (drawingPlayer.CursorDyeID != ID.None)
                {
                    var cursorDye = AQMod.CursorDyes.GetContent(drawingPlayer.CursorDyeID);
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
            if (CanApplyCursorDye() && OverrideColor)
            {
                ApplyNewColor();
                orig();
                ResetColor();
            }
            else
            {
                orig();
            }
        }
    }
}