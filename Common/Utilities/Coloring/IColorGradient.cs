using Microsoft.Xna.Framework;

namespace Aequus.Common.Utilities.Coloring
{
    public interface IColorGradient
    {
        Color GetColor(float time);
    }
}