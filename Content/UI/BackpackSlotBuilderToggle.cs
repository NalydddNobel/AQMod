using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.UI;

public class BackpackSlotBuilderToggle : BuilderToggle {
    public override bool Active() {
        return Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.extraInventory.Length > 0;
    }

    public override string DisplayValue() {
        return Language.GetTextValue("Mods.Aequus.Misc.BackpackSlots" + (CurrentState == 0 ? "On" : "Off"));
    }

    public override Color DisplayColorTexture() {
        return CurrentState == 0 ? Color.White : Color.Gray;
    }
}