using Aequus.Common.Players.Backpacks;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.ScavengerBag;

public class ScavengerBagBackpackData : BackpackItemData {
    public override bool SupportsInfoAccessories => true;

    public override Color SlotColor => new Color(160, 140, 255, 255);

    public override bool IsVisible() {
        return ModContent.GetInstance<ScavengerBagBuilderToggle>().CurrentState == 0;
    }
}