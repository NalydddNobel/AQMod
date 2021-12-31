using Terraria.ModLoader;

namespace AQMod.Content.DedicatedItemTags
{
    public interface IDedicationType
    {
        string Text { get; }
        void Draw(DrawableTooltipLine line);
    }
}