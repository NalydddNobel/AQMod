using AequusRemake.Content.Items.Accessories.GoldenFeather;
using System;
using Terraria.Localization;

namespace AequusRemake.Systems.CrossMod;

internal class Fargowiltas : SupportedMod<Fargowiltas> {
    private void AddStat(int itemDisplay, string statKey, Func<object> getStat) {
        LocalizedText text = this.GetLocalization("Stats." + statKey);

        Instance.Call("AddStat", itemDisplay, () => text.Format(getStat()));
    }

    public override void PostSetupContent() {
        AddStat(ModContent.ItemType<GoldenFeather>(), "RespawnReduction", () => ALanguage.Seconds(-Main.LocalPlayer.GetModPlayer<AequusPlayer>().respawnTimeModifierFlat));
    }
}