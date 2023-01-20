using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class AquaticEnergy : EnergyItemBase
    {
        protected override Vector3 LightColor => new Vector3(0.3f, 0.3f, 0.8f);
        public override int Rarity => ItemRarityID.Green;

        public override void AddRecipes()
        {

        }
    }
}