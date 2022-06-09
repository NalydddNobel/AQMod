using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class AtmosphericEnergy : EnergyItemBase
    {
        public static StaticMiscShaderInfo EnergyShader;
        public static Asset<Texture2D> AuraTexture;
        public override ref StaticMiscShaderInfo Shader => ref EnergyShader;
        public override ref Asset<Texture2D> Aura => ref AuraTexture;
        protected override Vector3 LightColor => new Vector3(0.65f, 0.65f, 0.2f);
        public override int Rarity => ItemRarityID.Orange;
    }
}