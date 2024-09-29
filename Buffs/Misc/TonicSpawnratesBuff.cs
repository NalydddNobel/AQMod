using Aequus.Common.DataSets;

namespace Aequus.Buffs.Misc;
public class TonicSpawnratesBuff : ModBuff {
    public override void SetStaticDefaults() {
        BuffSets.PotionPrefixBlacklist.Add(Type);
    }

    public override void Update(Player player, ref int buffIndex) {
        if (player.buffTime[buffIndex] <= 2 && !player.buffImmune[ModContent.BuffType<TonicSpawnratesDebuff>()]) {
            player.buffType[buffIndex] = ModContent.BuffType<TonicSpawnratesDebuff>();
            player.buffTime[buffIndex] = 1200;
        }
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}

public class TonicSpawnratesDebuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}