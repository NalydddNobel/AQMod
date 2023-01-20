using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class AtmosphericEnergy : EnergyItemBase
    {
        protected override Vector3 LightColor => new Vector3(0.65f, 0.65f, 0.2f);
        public override int Rarity => ItemRarityID.Pink;

        public override void AddRecipes()
        {

        }
    }
}