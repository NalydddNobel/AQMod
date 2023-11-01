using Aequus.Common.Players;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Inventory.ScavengerBag;

public class ScavengerBagBackpackData : BackpackItemData {
    public override bool SupportsInfoAccessories => true;

    public override Color SlotColor => new Color(160, 140, 255, 255);

    public override bool IsVisible() {
        return ModContent.GetInstance<ScavengerBagBuilderToggle>().CurrentState == 0;
    }
}