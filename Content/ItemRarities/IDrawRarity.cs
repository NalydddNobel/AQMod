using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.ItemRarities
{
    public interface IDrawRarity
    {
        void DrawTooltipLine(DrawableTooltipLine line)
        {
            DrawTooltipLine(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color));
        }
        void DrawTooltipLine(string text, int x, int y, Color color)
        {
            DrawTooltipLine(text, x, y, 0f, Vector2.Zero, Vector2.One, color);
        }
        void DrawTooltipLine(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color);
    }
}