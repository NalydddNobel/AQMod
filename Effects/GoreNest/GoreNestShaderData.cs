using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.GoreNest
{
    public class GoreNestShaderData : MiscShaderData
    {
        public GoreNestShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
        {
        }

        protected override void Apply()
        {
            _shader.Value.Parameters["colorLerpMult"].SetValue(0.45f + (float)Math.Sin(Main.GlobalTime * 10) * 0.1f);
            _shader.Value.Parameters["thirdColor"].SetValue(new Vector3(2f, 4f, 0));
            base.Apply();
        }
    }
}