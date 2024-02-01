using Aequus.Content.Equipment.Accessories.GoldenFeather;
using Aequus.Content.Equipment.Accessories.GrandReward;
using Aequus.Core.CrossMod;
using Aequus.Old.Content.Necromancy.Equipment.Accessories;
using Aequus.Old.Content.Necromancy.Equipment.Accessories.SpiritKeg;
using System;
using Terraria.Localization;

namespace Aequus.Content.CrossMod;

internal class Fargowiltas : SupportedMod<Fargowiltas> {
    private void AddStat(int itemDisplay, string statKey, Func<object> getStat) {
        LocalizedText text = this.GetLocalization("Stats." + statKey);

        Instance.Call("AddStat", itemDisplay, () => text.Format(getStat()));
    }

    public override void PostSetupContent() {
#if !DEBUG
        AddStat(ModContent.ItemType<BottleOSpirits>(), "MaxGhostSlots", () => Main.LocalPlayer.GetModPlayer<AequusPlayer>().ghostSlotsMax);
        AddStat(ModContent.ItemType<SaivoryKnife>(), "GhostDuration", () => ExtendLanguage.Minutes(Main.LocalPlayer.GetModPlayer<AequusPlayer>().ghostLifespan));
        AddStat(ModContent.ItemType<PandorasBox>(), "SceptreDebuffMultiplier", () => ExtendLanguage.Percent(Main.LocalPlayer.GetModPlayer<AequusPlayer>().zombieDebuffMultiplier));
        AddStat(ModContent.ItemType<GrandReward>(), "DropRolls", () => Main.LocalPlayer.GetModPlayer<AequusPlayer>().dropRolls);
        AddStat(ModContent.ItemType<GoldenFeather>(), "RespawnReduction", () => ExtendLanguage.Seconds(-Main.LocalPlayer.GetModPlayer<AequusPlayer>().respawnTimeModifierFlat));
#endif
    }
}