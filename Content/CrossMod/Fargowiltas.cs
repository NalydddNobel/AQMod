using Aequus.Content.Items.Accessories.GoldenFeather;
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
        AddStat(ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.SpiritKeg.SaivoryKnife>(), "GhostDuration", () => ExtendLanguage.Minutes(Main.LocalPlayer.GetModPlayer<AequusPlayer>().ghostLifespan.ApplyTo(Old.Content.Necromancy.NecromancySystem.DefaultLifespan)));
        AddStat(ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.PandorasBox>(), "SceptreDebuffMultiplier", () => ExtendLanguage.Percent(Main.LocalPlayer.GetModPlayer<AequusPlayer>().zombieDebuffMultiplier));
        AddStat(ModContent.ItemType<Old.Content.Items.Accessories.GrandReward.GrandReward>(), "DropRolls", () => Main.LocalPlayer.GetModPlayer<AequusPlayer>().dropRolls);
#endif
        AddStat(ModContent.ItemType<GoldenFeather>(), "RespawnReduction", () => XLanguage.Seconds(-Main.LocalPlayer.GetModPlayer<AequusPlayer>().respawnTimeModifierFlat));
    }
}