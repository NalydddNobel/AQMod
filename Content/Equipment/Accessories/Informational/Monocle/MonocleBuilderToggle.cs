using Terraria.Localization;

namespace Aequus.Content.Equipment.Accessories.Informational.Monocle;

public class MonocleBuilderToggle : BuilderToggle {
    public override System.Boolean Active() {
        return Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.accMonocle;
    }

    public override System.String DisplayValue() {
        return Language.GetTextValue("Mods.Aequus.Misc.MonocleToggle" + (CurrentState == 0 ? "On" : "Off"));
    }

    public override Color DisplayColorTexture() {
        return CurrentState == 0 ? Color.White : Color.Gray;
    }
}