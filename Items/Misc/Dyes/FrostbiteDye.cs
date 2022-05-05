using Aequus.Graphics.ArmorShaders;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace Aequus.Items.Misc.Dyes
{
    public class FrostbiteDye : DyeItemBase
    {
        public override string Pass => "HoriztonalWavePass";
        public override int Rarity => ItemDefaults.RarityGaleStreams - 1;

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataModifyLightColor(Effect, Pass, (v) =>
            {
                return v * new Vector3(0.05f, 0.2f, 0.9f);
            }).UseColor(new Vector3(0.05f, 0.2f, 0.9f)).UseSecondaryColor(new Vector3(0.1f, 0.4f, 2f));
        }
    }
}