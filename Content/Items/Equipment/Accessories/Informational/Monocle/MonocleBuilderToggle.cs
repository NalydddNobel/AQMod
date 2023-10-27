using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Informational.Monocle;

public class MonocleBuilderToggle : BuilderToggle {
    public override bool Active() {
        return Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.accMonocle;
    }

    public override string DisplayValue() {
        return Language.GetTextValue("Mods.Aequus.Misc.MonocleToggle" + (CurrentState == 0 ? "On" : "Off"));
    }

    public override Color DisplayColorTexture() {
        return CurrentState == 0 ? Color.White : Color.Gray;
    }
}