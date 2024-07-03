using Aequu2.Content.Items.Accessories.GoldenFeather;
using Aequu2.Core.CrossMod;
using System;
using Terraria.Localization;

namespace Aequu2.Content.CrossMod;

internal class Fargowiltas : SupportedMod<Fargowiltas> {
    private void AddStat(int itemDisplay, string statKey, Func<object> getStat) {
        LocalizedText text = this.GetLocalization("Stats." + statKey);

        Instance.Call("AddStat", itemDisplay, () => text.Format(getStat()));
    }

    public override void PostSetupContent() {
#if !DEBUG
        AddStat(ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.SpiritKeg.BottleOSpirits>(), "MaxGhostSlots", () => Main.LocalPlayer.GetModPlayer<Aequu2Player>().ghostSlotsMax);
        AddStat(ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.SpiritKeg.SaivoryKnife>(), "GhostDuration", () => XLanguage.Minutes(Main.LocalPlayer.GetModPlayer<Aequu2Player>().ghostLifespan.ApplyTo(Old.Content.Necromancy.NecromancySystem.DefaultLifespan)));
        AddStat(ModContent.ItemType<Old.Content.Necromancy.Equipment.Accessories.PandorasBox>(), "SceptreDebuffMultiplier", () => XLanguage.Percent(Main.LocalPlayer.GetModPlayer<Aequu2Player>().zombieDebuffMultiplier));
        AddStat(ModContent.ItemType<Old.Content.Items.Accessories.GrandReward.GrandReward>(), "DropRolls", () => Main.LocalPlayer.GetModPlayer<Aequu2Player>().dropRolls);
#endif
        AddStat(ModContent.ItemType<GoldenFeather>(), "RespawnReduction", () => XLanguage.Seconds(-Main.LocalPlayer.GetModPlayer<AequusPlayer>().respawnTimeModifierFlat));
    }
}