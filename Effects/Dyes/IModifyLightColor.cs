using Microsoft.Xna.Framework;

namespace AQMod.Effects.Dyes
{
    public interface IModifyLightColor
    {
        Vector3 ModifyLightColor(Vector3 light);
    }
}