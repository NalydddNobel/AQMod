using Terraria.Localization;

namespace Aequus.Old.Content.Equipment.GrapplingHooks.HealingGrappleHook;

public class LeechHookDebuff : ModBuff {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
    }

    public override LocalizedText DisplayName => Language.GetText(this.GetLocalizationKey("DisplayName"));
    public override LocalizedText Description => LocalizedText.Empty;
}
