using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Material.Energy; 

public class AtmosphericEnergy : EnergyItemBase {
    protected override Vector3 LightColor => new Vector3(0.35f, 0.3f, 0.1f);
    public override int Rarity => ItemRarityID.Pink;

    public override void AddRecipes() {
    }
}