using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects
{
    public class DynamicColorArmorShaderData : ArmorShaderData
    {
        public DynamicColorArmorShaderData(Ref<Effect> shader, string passName, Func<Vector3> getColor) : base(shader, passName)
        {
            _getColor = getColor;
        }

        protected Func<Vector3> _getColor;

        protected override void Apply()
        {
            _shader.Value.Parameters["uColor"].SetValue(_getColor());
            base.Apply();
        }
    }
}