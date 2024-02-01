using Aequus.Content.Equipment.Accessories.GoldenFeather;
using Aequus.Content.Equipment.Accessories.GrandReward;
using Aequus.Core.CrossMod;
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
        AddStat(ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.SpiritKeg.BottleOSpirits>(), "MaxGhostSlots", () => Main.LocalPlayer.GetModPlayer<AequusPlayer>().ghostSlotsMax);
        AddStat(ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.SpiritKeg.SaivoryKnife>(), "GhostDuration", () => ExtendLanguage.Minutes(Main.LocalPlayer.GetModPlayer<AequusPlayer>().ghostLifespan));
        AddStat(ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.PandorasBox>(), "SceptreDebuffMultiplier", () => ExtendLanguage.Percent(Main.LocalPlayer.GetModPlayer<AequusPlayer>().zombieDebuffMultiplier));
#endif
        AddStat(ModContent.ItemType<GrandReward>(), "DropRolls", () => Main.LocalPlayer.GetModPlayer<AequusPlayer>().dropRolls);
        AddStat(ModContent.ItemType<GoldenFeather>(), "RespawnReduction", () => ExtendLanguage.Seconds(-Main.LocalPlayer.GetModPlayer<AequusPlayer>().respawnTimeModifierFlat));
    }
}