using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Aequus.Graphics.ArmorShaders
{
    public class ArmorShaderDataHellBeams : ArmorShaderDataLightSourceAsColor, IShaderDataModifyLightColor
    {
        public ArmorShaderDataHellBeams(Ref<Effect> shader, string passName, Vector3 thirdColor) : base(shader, passName, thirdColor)
        {
        }

        Vector3 IShaderDataModifyLightColor.ModifyLightColor(Vector3 light)
        {
            return light * new Vector3(1f, 0.8f, 0f);
        }
    }
}