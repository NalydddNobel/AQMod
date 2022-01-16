using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.Players
{
    public sealed class PlayerCursorDyes : ModPlayer
    {
        public static byte LocalCursorDye =>
            (byte)((Main.myPlayer == -1 || Main.player[Main.myPlayer] == null) ? 0 : Main.player[Main.myPlayer].GetModPlayer<PlayerCursorDyes>().VisibleCursorDye);

        public byte cursorDye;
        public byte VisibleCursorDye;

        public override void Initialize()
        {
            cursorDye = 0;
            VisibleCursorDye = 0;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["cursorDye"] = cursorDye,
            };
        }

        public override void Load(TagCompound tag)
        {
            cursorDye = tag.GetByte("cursorDye");
        }

        public override void ResetEffects()
        {
            VisibleCursorDye = cursorDye;
        }
    }
}