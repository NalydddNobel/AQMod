using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.Dyes
{
    public sealed class ArmorShaderDataDynamicColor : ArmorShaderData
    {
        private Func<Color> getColor;

        public ArmorShaderDataDynamicColor(Ref<Effect> shader, string passName, Func<Color> func) : base(shader, passName)
        {
            getColor = func;
        }

        protected override void Apply()
        {
            UseColor(getColor());
            base.Apply();
        }
    }
}
