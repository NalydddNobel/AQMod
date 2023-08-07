using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.Energies {
    [LegacyName("LightMatter")]
    public class CosmicEnergy : EnergyItemBase {
        public override int Rarity => ItemRarityID.LightRed;
        protected override Vector3 LightColor => new Vector3(0.3f, 0.3f, 0.8f);

        public override void AddRecipes() {
        }
    }
}