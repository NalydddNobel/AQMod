using Aequus.Common;
using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class DemonicEnergy : BaseEnergy
    {
        protected override IColorGradient Gradient => ColorHelper.Instance.DemonicGrad;
        protected override Vector3 LightColor => new Vector3(0.8f, 0.2f, 0.2f);
        public override int Rarity => ItemRarityID.Orange;
    }
}