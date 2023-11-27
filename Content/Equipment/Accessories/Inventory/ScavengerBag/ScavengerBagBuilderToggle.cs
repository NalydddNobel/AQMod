using Aequus.Common.Players.Backpacks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.Inventory.ScavengerBag;

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