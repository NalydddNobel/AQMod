using Microsoft.Xna.Framework;

namespace Aequus.Graphics.ArmorShaders
{
    public interface IShaderDataModifyLightColor
    {
        Vector3 ModifyLightColor(Vector3 light);
    }
}