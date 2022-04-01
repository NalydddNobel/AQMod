using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Aequus.Assets.Effects.Armor
{
    public class ArmorShaderDataHellBeams : ArmorShaderDataLightSourceAsColor, IModifyLightColor
    {
        public ArmorShaderDataHellBeams(Ref<Effect> shader, string passName, Vector3 thirdColor) : base(shader, passName, thirdColor)
        {
        }

        Vector3 IModifyLightColor.ModifyLightColor(Vector3 light)
        {
            return light * new Vector3(1f, 0.8f, 0f);
        }
    }
}