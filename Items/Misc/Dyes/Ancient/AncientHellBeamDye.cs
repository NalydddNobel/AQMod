using Aequus.Common.Effects.ShaderData;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes.Ancient {
    public class AncientHellBeamDye : DyeItemBase
    {
        public override int Rarity => ItemRarityID.Green;

        public override string Pass => "ShieldBeamsPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataThirdColor(Effect, Pass,
                new Vector3(0.3f, 0.2f, 0f)).UseColor(new Vector3(1f, 0.8f, 0.1f)).UseSecondaryColor(1.8f, 0.8f, 0.6f).UseOpacity(5f).UseSaturation(1f);
        }
    }
}