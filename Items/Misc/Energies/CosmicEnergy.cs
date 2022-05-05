using Aequus.Common;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class CosmicEnergy : EnergyItemBase
    {
        public override int Rarity => ItemRarityID.Green;
        protected override IColorGradient Gradient => Gradients.cosmicGrad;
        protected override Vector3 LightColor => new Vector3(0.3f, 0.3f, 0.8f);
    }
}