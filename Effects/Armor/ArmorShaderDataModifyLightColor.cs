using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Aequus.Effects.Armor
{
    public class ArmorShaderDataModifyLightColor : ArmorShaderData, IShaderDataModifyLightColor
    {
        private readonly Func<Vector3, Vector3> _modifyColor;

        public ArmorShaderDataModifyLightColor(Ref<Effect> shader, string passName, Vector3 modifyColor) : base(shader, passName)
        {
            _modifyColor = (v) => v * modifyColor;
        }

        public ArmorShaderDataModifyLightColor(Ref<Effect> shader, string passName, Func<Vector3, Vector3> modifyColor) : base(shader, passName)
        {
            _modifyColor = modifyColor;
        }

        public Vector3 ModifyLightColor(Vector3 light)
        {
            return _modifyColor(light);
        }
    }
}