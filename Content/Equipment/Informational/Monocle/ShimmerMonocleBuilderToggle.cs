using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Informational.Monocle;

public class ShimmerMonocleBuilderToggle : BuilderToggle {
    public override bool Active() {
        return Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.accInfoShimmerMonocle;
    }

    public override string DisplayValue() {
        return Language.GetTextValue("Mods.Aequus.Misc.ShimmerMonocleToggle" + (CurrentState == 0 ? "On" : "Off"));
    }

    public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
        drawParams.Color = CurrentState == 0 ? Color.White : Color.Gray;
        return true;
    }
}