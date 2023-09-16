using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Material.Energy;

public class OrganicEnergy : EnergyItemBase {
    protected override Vector3 LightColor => new Vector3(0.2f, 0.7f, 0.1f);
    public override int Rarity => ItemRarityID.Lime;

    public override void AddRecipes() {
    }
}