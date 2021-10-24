using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.CursorDyes
{
    public sealed class CursorDyeHealth : CursorDye
    {
        public CursorDyeHealth(Mod mod, string name) : base(mod, name)
        {
        }

        public override bool ApplyColor(Player player, AQPlayer drawingPlayer, out Color newCursorColor)
        {
            newCursorColor = Color.Lerp(new Color(20, 1, 1, 255), new Color(255, 40, 40, 255), player.statLife / (float)player.statLifeMax2);
            return true;
        }
    }
}