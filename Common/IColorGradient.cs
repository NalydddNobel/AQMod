using Microsoft.Xna.Framework;

namespace Aequus.Common
{
    public interface IColorGradient
    {
        Color GetColor(float time);
    }
}