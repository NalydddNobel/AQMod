using Aequus.Common.Items;
using Aequus.Core.CodeGeneration;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Informational.DebuffDPSMeter;

[InfoPlayerField("accInfoDebuffDPS", "bool")]
public class GeigerCounter : ModItem {
    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.LifeformAnalyzer);
    }

    public override void UpdateInfoAccessory(Player player) {
        if (PDAEffects.PDAUpgrades.Contains(Type)) {

        }
        player.GetModPlayer<AequusPlayer>().accInfoDebuffDPS = true;
    }
}

public class GeigerCounterInfoDisplay : InfoDisplay {
    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        int dps = GetDPS(Main.myPlayer);

        if (dps > 0) {
            return Language.GetTextValue("GameUI.DPS", dps);
        }

        displayColor = InactiveInfoTextColor;
        return Language.GetTextValue("GameUI.NoDPS");
    }

    private static int GetDPS(int plr) {
        double dps = 0.0;
        Vector2 playerPosition = Main.player[plr].Center;

        for (int i = 0; i < Main.maxNPCs; i++) {
            NPC npc = Main.npc[i];
            if (npc.active && npc.lifeRegen < 0 && npc.playerInteraction[plr]) {
                dps -= npc.lifeRegen / 2.0;
            }
        }

        return (int)dps;
    }

    public override bool Active() {
        return Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoDebuffDPS;
    }
}
