using Terraria.DataStructures;
using Terraria.Localization;

namespace AequusRemake.Content.Items.Accessories.Informational.Monocle;

public class MonocleBuilderToggle : BuilderToggle {
    public override bool Active() {
        return Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var AequusRemakePlayer) && AequusRemakePlayer.accInfoMoneyMonocle;
    }

    public override string DisplayValue() {
        return Language.GetTextValue("Mods.AequusRemake.Misc.MonocleToggle" + (CurrentState == 0 ? "On" : "Off"));
    }

    public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
        drawParams.Color = CurrentState == 0 ? Color.White : Color.Gray;
        return true;
    }
}