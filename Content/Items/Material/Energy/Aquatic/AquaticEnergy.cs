using Microsoft.Xna.Framework;

namespace Aequus.Content.Items.Material.Energy.Aquatic;

public class AquaticEnergy : EnergyItemBase<AquaticEnergyParticle> {
    protected override Vector3 LightColor => new Vector3(0.2f, 0.4f, 0.8f);
    public override int Rarity => ItemRarityID.Blue;
}