using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Material.Energy; 

public class DemonicEnergy : EnergyItemBase {
    protected override Vector3 LightColor => new Vector3(1f, 0.1f, 0.1f);
    public override int Rarity => ItemRarityID.Orange;

    public override void AddRecipes() {
    }
}