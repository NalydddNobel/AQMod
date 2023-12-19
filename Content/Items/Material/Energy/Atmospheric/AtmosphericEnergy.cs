using Microsoft.Xna.Framework;

namespace Aequus.Content.Items.Material.Energy.Atmospheric;

public class AtmosphericEnergy : EnergyItemBase<AtmosphericEnergyParticle> {
    protected override Vector3 LightColor => new Vector3(0.35f, 0.3f, 0.1f);
    public override int Rarity => ItemRarityID.Pink;

    public override void AddRecipes() {
    }
}