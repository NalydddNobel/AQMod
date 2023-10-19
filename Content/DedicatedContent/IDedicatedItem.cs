using Microsoft.Xna.Framework;

namespace Aequus.Content.DedicatedContent;

public interface IDedicatedItem {
    string DisplayedDedicateeName { get; }
    Color TextColor { get; }
    public Color FaelingColor => TextColor;
}