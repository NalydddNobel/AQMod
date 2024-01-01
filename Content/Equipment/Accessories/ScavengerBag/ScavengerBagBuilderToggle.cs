using Aequus.Common.Players.Backpacks;
using Microsoft.Xna.Framework;

namespace Aequus.Content.Equipment.Accessories.ScavengerBag;

public class ScavengerBagBuilderToggle : BuilderToggle {
    public override bool Active() {
        return BackpackLoader.Get<ScavengerBagBackpackData>(Main.LocalPlayer).IsActive(Main.LocalPlayer);
    }

    public override string DisplayValue() {
        return ModContent.GetInstance<ScavengerBagBackpackData>().GetBuilderSlotText(CurrentState);
    }

    public override Color DisplayColorTexture() {
        return CurrentState == 0 ? Color.White : Color.Gray;
    }
}