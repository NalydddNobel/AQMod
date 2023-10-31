using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Inventory.ScavengerBag;

public class ScavengerBagBuilderToggle : BuilderToggle {
    public override bool Active() {
        return Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.accScavengerBag != null;
    }

    public override string DisplayValue() {
        return ModContent.GetInstance<ScavengerBagBackpackData>().GetBuilderSlotText(CurrentState);
    }

    public override Color DisplayColorTexture() {
        return CurrentState == 0 ? Color.White : Color.Gray;
    }
}