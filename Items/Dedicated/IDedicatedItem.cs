using Microsoft.Xna.Framework;

namespace AQMod.Items.Dedicated
{
    public interface IDedicatedItem
    {
        IDedicationType DedicationType { get; }
        Color DedicatedItemColor { get; }
    }
}