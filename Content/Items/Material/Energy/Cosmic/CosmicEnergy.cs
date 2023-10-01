using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Material.Energy.Cosmic;

[LegacyName("LightMatter")]
public class CosmicEnergy : EnergyItemBase<CosmicEnergyParticle> {
    public override int Rarity => ItemRarityID.LightRed;
    protected override Vector3 LightColor => new Vector3(0.3f, 0.3f, 0.8f);

    public override void AddRecipes() {
    }
}