using Microsoft.Xna.Framework;

namespace Aequus.Content.Items.Material.Energy.Demonic;

public class DemonicEnergy : EnergyItemBase<DemonicEnergyParticle> {
    protected override Vector3 LightColor => new Vector3(1f, 0.1f, 0.1f);
    public override int Rarity => ItemRarityID.Orange;

    public override void AddRecipes() {
    }
}