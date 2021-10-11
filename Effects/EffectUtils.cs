using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects
{
    public static class EffectUtils
    {
        public static void UseImageSize(this MiscShaderData data, Vector2 imageSize)
        {
            data.Shader.Parameters["uImageSize0"].SetValue(imageSize);
        }
    }
}