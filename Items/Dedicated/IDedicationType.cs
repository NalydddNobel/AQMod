using Terraria.ModLoader;

namespace AQMod.Items.Dedicated
{
    public interface IDedicationType
    {
        string Text { get; }
        void Draw(DrawableTooltipLine line);
    }
}