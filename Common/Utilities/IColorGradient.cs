using Microsoft.Xna.Framework;

namespace Aequus.Common.Utilities
{
    public interface IColorGradient
    {
        Color GetColor(float time);
    }
}