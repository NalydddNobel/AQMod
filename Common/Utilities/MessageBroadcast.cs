using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AQMod.Common.Utilities
{
    public static class MessageBroadcast
    {
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
                Broadcast(key, r, g, b);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(key), r, g, b);
            }
        }
        internal static void Broadcast(string key, byte r, byte g, byte b)
        {
            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), new Color(r, g, b, 255));
        }
    }
}