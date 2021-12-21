using AQMod.Common;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.Dyes
{
    public sealed class DyeHelper : IAutoloadType
    {
        private static FieldInfo _armorShaderDataColorField;
        private static FieldInfo _shaderDataPassField;

        private static class Errors
        {
            public static byte _passNameError;
        }

        void IAutoloadType.OnLoad()
        {
            _armorShaderDataColorField = typeof(ArmorShaderData).GetField("_uColor", BindingFlags.NonPublic | BindingFlags.Instance);
            _shaderDataPassField = typeof(ShaderData).GetField("_passName", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        void IAutoloadType.Unload()
        {
            _armorShaderDataColorField = null;
            _shaderDataPassField = null;
        }

        public static Color ModifyLight(ArmorShaderData dye, Color light)
        {
            var clr = ModifyLight(dye, light.ToVector3());
            return new Color(clr.X, clr.Y, clr.Z, light.A / 255f);
        }

        public static Vector3 ModifyLight(ArmorShaderData dye, Vector3 light)
        {
            string passName = "";
            if (dye is IModifyLightColor modifyLight)
            {
                return modifyLight.ModifyLightColor(light);
            }
            if (Errors._passNameError < 200)
            {
                try
                {
                    passName = (string)_shaderDataPassField.GetValue(dye);
                }
                catch
                {
                    Errors._passNameError += 10;
                }
            }
            if (passName != "") // can't really detect the shader being used, so if a custom shader has any of these keys, it's kinda screwed?
            {
                switch (passName)
                {
                    case "ArmorColored": 
                    {
                        var shaderColor = (Vector3)_armorShaderDataColorField.GetValue(dye);
                        return light * shaderColor;
                    }
                }
            }
            return light;
        }
    }
}