using Microsoft.Xna.Framework;

namespace AQMod.Content.DedicatedItemTags
{
    public interface IDedicatedItem
    {
        IDedicationType DedicationType { get; }
        Color DedicatedItemColor { get; }
    }
}