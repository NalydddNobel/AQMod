using AQMod.Common;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AQMod
{
    internal static class AQUtils
    {
        public static class OmegaStariteHelper
        {
            public static Vector2 ViewCenter;
            public static void ScreenView()
            {
                ViewCenter = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
            }
            public static void WorldView()
            {
                ViewCenter = new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f);
            }

            public const float Z_VIEW = -20f;

            internal static Vector2 GetParralaxPosition(Vector2 origin, float z)
            {
                z = MultZ(z);
                return new Vector2(origin.X - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.X - ViewCenter.X), origin.Y - (1f - (-Z_VIEW / (z - Z_VIEW))) * (origin.Y - ViewCenter.Y));
            }

            public static float GetParralaxScale(float originalScale, float z)
            {
                z = MultZ(z);
                return originalScale * (-Z_VIEW / (z - Z_VIEW));
            }

            public static float MultZ(float z)
            {
                return MultZ(z, ClientOptions.Instance);
            }

            public static float MultZ(float z, ClientOptions options)
            {
                return z *= options.OmegaStarite3DPronunciation + 0.01f; // adding 0.001 because some things actually rely on z for layering
            }
        }

        internal static void BroadcastMessage(string key, Color color)
        {
            Main.NewText(Language.GetTextValue(key), color);
            //if (Main.netMode == NetmodeID.Server)
            //{
                
            //    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), color);
            //}
            //else if (Main.netMode == NetmodeID.SinglePlayer)
            //{
            //}
        }

        public static string GetPath<T>()
        {
            return GetPath(typeof(T));
        }

        public static string GetPath<T>(string extra)
        {
            return GetPath(typeof(T), extra);
        }

        public static string GetPath(this object o)
        {
            return GetPath(o.GetType());
        }

        public static string GetPath(this object o, string extra)
        {
            return GetPath(o.GetType(), extra);
        }

        public static string GetPath(Type t)
        {
            return t.Namespace.Replace('.', '/') + "/" + t.Name;
        }

        public static string GetPath(Type t, string extra)
        {
            return GetPath(t) + extra;
        }

        public static Color ShiftThroughMultipleColors(Color[] colors, float time)
        {
            int index = (int)time;
            return Color.Lerp(colors[index % colors.Length], colors[(index + 1) % colors.Length], time % 1f);
        }

        public static bool IsCloseEnoughTo(this float comparison, float intendedValue, float closeEnoughMargin = 1f)
        {
            return (comparison - intendedValue).Abs() <= closeEnoughMargin;
        }

        public static float Abs(this float value)
        {
            return value < 0f ? -value : value;
        }
    }
}
