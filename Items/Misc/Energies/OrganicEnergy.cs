using Aequus.Common;
using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class OrganicEnergy : BaseEnergy
    {
        protected override IColorGradient Gradient => ColorHelper.Instance.OrganicGrad;
        protected override Vector3 LightColor => new Vector3(0.3f, 0.75f, 0.2f);
        public override int Rarity => ItemRarityID.Green;
    }
}