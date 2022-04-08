using Microsoft.Xna.Framework;

namespace Aequus.Effects.Armor
{
    public interface IShaderDataModifyLightColor
    {
        Vector3 ModifyLightColor(Vector3 light);
    }
}