using Aequu2.DataSets;
using Terraria.Localization;

namespace Aequu2.Content.Items.Potions.Buffs.Sentry;

public class SentryBuff : ModBuff {
    public override LocalizedText DisplayName => ModContent.GetInstance<SentryPotion>().GetLocalization("BuffName");
    public override LocalizedText Description => ModContent.GetInstance<SentryPotion>().Tooltip;

    public override void SetStaticDefaults() {
        BuffDataSet.RegisterConflict(Type, BuffID.Summoning);
    }

    public override void Update(Player player, ref int buffIndex) {
        player.maxTurrets++;
    }
}