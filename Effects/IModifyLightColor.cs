using Microsoft.Xna.Framework;

namespace Aequus.Effects
{
    public interface IModifyLightColor
    {
        Vector3 ModifyLightColor(Vector3 light);
    }
}