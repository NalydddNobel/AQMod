using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Misc.Energies
{
    public class OrganicEnergy : EnergyItemBase
    {
        protected override Vector3 LightColor => new Vector3(0.2f, 0.5f, 0.3f);
        public override int Rarity => ItemRarityID.Lime;

        public override void AddRecipes()
        {

        }
    }
}