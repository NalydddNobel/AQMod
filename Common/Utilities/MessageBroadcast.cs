using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AQMod.Common.Utilities
{
    public static class MessageBroadcast
    {
        public static class Hooks
        {
            internal static void NetMessage_BroadcastChatMessage(On.Terraria.NetMessage.orig_BroadcastChatMessage orig, NetworkText text, Color color, int excludedPlayer)
            {
                if (PreventChatOnce)
                {
                    PreventChatOnce = false;
                    return;
                }
                if (PreventChat)
                {
                    return;
                }
                orig(text, color, excludedPlayer);
            }
            internal static void Main_NewText_string_byte_byte_byte_bool(On.Terraria.Main.orig_NewText_string_byte_byte_byte_bool orig, string newText, byte R, byte G, byte B, bool force)
            {
                if (PreventChatOnce)
                {
                    PreventChatOnce = false;
                    return;
                }
                if (PreventChat)
                {
                    return;
                }
                orig(newText, R, G, B, force);
            }
        }

        public static bool PreventChat { get; internal set; }
        public static bool PreventChatOnce { get; internal set; }

        internal static void NewMessage(string key)
        {
            NewMessage(key, 255, 255, 255);
        }
        internal static void NewMessage(string key, Color color)
        {
            NewMessage(key, color.R, color.G, color.B);
        }
        internal static void NewMessage(string key, byte r, byte g, byte b)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), new Color(r, g, b, 255));
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(key), r, g, b);
            }
        }
    }
}