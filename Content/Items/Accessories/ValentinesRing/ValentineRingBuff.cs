using AequusRemake.Core.Util.Helpers;
using System.Collections.Generic;
using Terraria.Localization;

namespace AequusRemake.Content.Items.Accessories.ValentinesRing;

public class ValentineRingBuff : ModBuff {
    public override LocalizedText DisplayName => ModContent.GetInstance<ValentineRing>().GetLocalization("BuffName");
    public override LocalizedText Description => ModContent.GetInstance<ValentineRing>().GetLocalization("BuffDescription");
    public LocalizedText ExtraDescription { get; private set; }

    public override void SetStaticDefaults() {
        ExtraDescription = ModContent.GetInstance<ValentineRing>().GetLocalization("BuffDescription2");

        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.lifeRegen += ValentineRing.LifeRegenerationAmount;
        player.GetDamage(DamageClass.Generic) += ValentineRing.DamageAmount;
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) {
        string addTip = "";
        foreach (Player p in GetBuffingPlayers(Main.LocalPlayer)) {
            if (!string.IsNullOrEmpty(addTip)) {
                addTip += ", & ";
            }

            addTip += ColorTagProvider.Color(Color.Pink, p.name);
        }

        if (!string.IsNullOrEmpty(addTip)) {
            tip += '\n' + ExtraDescription.Format(addTip);
        }
    }

    private static IEnumerable<Player> GetBuffingPlayers(Player playerToBuff) {
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && Main.player[i].GetModPlayer<AequusPlayer>().accGifterRing == playerToBuff.name) {
                yield return Main.player[i];
            }
        }
    }
}
