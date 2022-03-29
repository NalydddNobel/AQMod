using Microsoft.Xna.Framework;

namespace Aequus.Assets.Effects.Prims
{
    public interface IPrimRenderer
    {
        void Draw(Vector2[] arr, float uvAdd = 0f, float uvMultiplier = 1f);
    }
}