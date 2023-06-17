using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Materials.Energies {
    public class AtmosphericEnergy : EnergyItemBase {
        protected override Vector3 LightColor => new Vector3(0.45f, 0.2f, 0.1f);
        public override int Rarity => ItemRarityID.Pink;

        public override void AddRecipes() {
        }
    }
}