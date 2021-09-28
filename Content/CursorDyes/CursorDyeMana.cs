using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public sealed class CursorDyeMana : CursorDye
    {
        public CursorDyeMana(Mod mod, string name) : base(mod, name)
        {
        }

        public override bool ApplyColor(Player player, GraphicsPlayer drawingPlayer, out Color newCursorColor)
        {
            newCursorColor = Color.Lerp(new Color(255, 255, 255, 255), new Color(40, 40, 255, 255), player.statMana / (float)player.statManaMax2);
            return true;
        }
    }
}