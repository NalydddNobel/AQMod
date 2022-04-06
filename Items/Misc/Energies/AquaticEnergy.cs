using Aequus.Common.Utilities.Coloring;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class AquaticEnergy : BaseEnergy
    {
        protected override IColorGradient Gradient => ColorHelper.Instance.AquaticGrad;
        protected override Vector3 LightColor => new Vector3(0.3f, 0.3f, 0.8f);
        public override int Rarity => ItemRarityID.Blue;
    }
}