using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Nettlebane;

public class NettlebaneBuffTier1 : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.statDefense += 12;
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}

public class NettlebaneBuffTier2 : NettlebaneBuffTier1 {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Main.buffNoTimeDisplay[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.statDefense += 18;
    }

    public override bool RightClick(int buffIndex) {
        return true;
    }
}

public class NettlebaneBuffTier3 : NettlebaneBuffTier2 {
    public override void Update(Player player, ref int buffIndex) {
        player.statDefense += 24;
        player.ClearBuff(ModContent.BuffType<NettlebaneBuffTier2>());
    }

    public override bool RightClick(int buffIndex) {
        var player = Main.LocalPlayer;
        player.AddBuff(ModContent.BuffType<NettlebaneBuffTier2>(), player.buffTime[buffIndex]);
        return true;
    }
}