using Aequus.Common.NPCs.Global;
using Terraria.Localization;

namespace Aequus.Content.Items.GrapplingHooks.Meathook;

public class MeathookDebuff : ModBuff {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
    }

    public override LocalizedText DisplayName => ModContent.GetInstance<Meathook>().DisplayName;
    public override LocalizedText Description => LocalizedText.Empty;

    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.TryGetGlobalNPC(out StatSpeedGlobalNPC speed)) {
            speed.statSpeed *= Meathook.SlowTargetMultiplier;
        }
    }
}