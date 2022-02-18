using Microsoft.Xna.Framework;

namespace AQMod.Common.Utilities.Colors
{
    public interface IColorGradient
    {
        Color GetColor(float time);
    }
}