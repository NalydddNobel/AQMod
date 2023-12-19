using Microsoft.Xna.Framework;

namespace Aequus.Content.Items.Material.Energy.Organic;

public class OrganicEnergy : EnergyItemBase<OrganicEnergyParticle> {
    protected override Vector3 LightColor => new Vector3(0.2f, 0.7f, 0.1f);
    public override int Rarity => ItemRarityID.Lime;

    public override void AddRecipes() {
    }
}