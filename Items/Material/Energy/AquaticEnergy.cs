using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Material.Energy; 

public class AquaticEnergy : EnergyItemBase {
    protected override Vector3 LightColor => new Vector3(0.2f, 0.4f, 0.8f);
    public override int Rarity => ItemRarityID.Green;
}