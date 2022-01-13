using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.Players
{
    public sealed class PlayerCursorDyes : ModPlayer
    {
        public static byte LocalCursorDye = (byte)(Main.myPlayer == -1 || Main.player[Main.myPlayer] == null ? 0 : Main.player[Main.myPlayer].GetModPlayer<PlayerCursorDyes>().cursorDye);

        public byte cursorDye;
    }
}