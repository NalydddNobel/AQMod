using Microsoft.Xna.Framework;

namespace Aequus.Graphics.ShaderData
{
    public interface IShaderDataModifyLightColor
    {
        Vector3 ModifyLightColor(Vector3 light);
    }
}