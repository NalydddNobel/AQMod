using Aequus.Graphics.ShaderData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes
{
    public class HellBeamDye : DyeItemBase
    {
        public override int Rarity => ItemRarityID.Green;

        public override string Pass => "ShieldBeamsPass";

        public class ArmorShaderDataHellBeams : ArmorShaderDataThirdColor, IShaderDataModifyLightColor
        {
            public ArmorShaderDataHellBeams(Ref<Effect> shader, string passName, Vector3 thirdColor) : base(shader, passName, thirdColor)
            {
            }

            Vector3 IShaderDataModifyLightColor.ModifyLightColor(Vector3 light)
            {
                return light * new Vector3(1f, 0.8f, 0f);
            }
        }

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataHellBeams(Effect, Pass,
                new Vector3(0.3f, 0.2f, 0f)).UseColor(new Vector3(1f, 0.8f, 0.1f)).UseSecondaryColor(1.8f, 0.8f, 0.6f).UseOpacity(5f).UseSaturation(1f);
        }
    }
}