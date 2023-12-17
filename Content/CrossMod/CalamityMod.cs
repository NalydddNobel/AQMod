using Aequus.Common.Players.Stats;
using Aequus.Core.CrossMod;
using System;
using Terraria.Localization;

namespace Aequus.Content.CrossMod;

internal class CalamityMod : ModSupport<CalamityMod> {
    public override void PostSetupContent() {
        StatCompareLoader.Register(new ComparePercent((p) => TryCall(out float value, "GetRogueVelocity", p) ? value : 0f, Affix("RogueVelocity")));
        StatCompareLoader.Register(new CompareFloat((p) => TryCall(out float value, "GetMaxStealth", p) ? value * 100f : 0f, Affix("MaxStealth")));

        static LocalizedText Affix(string key, Func<string> createDefault = null) {
            return StatCompareLoader.Affix("CalamityMod." + key, createDefault);
        }
    }
}