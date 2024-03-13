using Aequus.Common.Buffs.Components;

namespace Aequus.Common.Buffs;
public partial class AequusBuff : GlobalBuff {
    public override void Load() {
        On_NPC.AddBuff += NPC_AddBuff;
        On_Player.AddBuff += Player_AddBuff;
        On_Player.QuickBuff_ShouldBotherUsingThisBuff += On_Player_QuickBuff_ShouldBotherUsingThisBuff;
    }

    private static bool On_Player_QuickBuff_ShouldBotherUsingThisBuff(On_Player.orig_QuickBuff_ShouldBotherUsingThisBuff orig, Player self, int attemptedType) {
        if (BuffLoader.GetBuff(attemptedType) is ICheckQuickBuff modifyQuickBuff && !modifyQuickBuff.CheckQuickBuff(self)) {
            return false;
        }

        return orig(self, attemptedType);
    }

    #region Hooks
    private static void NPC_AddBuff(On_NPC.orig_AddBuff orig, NPC npc, int type, int time, bool quiet) {
        var onAddBuff = BuffLoader.GetBuff(type) as IOnAddBuff;
        onAddBuff?.PreAddBuff(npc, ref time, ref quiet);
        //if (Main.debuff[type] || BuffSets.ProbablyFireDebuff.Contains(type)) {
        //    var player = AequusPlayer.CurrentPlayerContext();
        //    if (player != null) {
        //        var aequus = player.Aequus();
        //        time = (int)(time * aequus.DebuffsInfliction.GetBuffMultipler(player, type));
        //        if (aequus.accResetEnemyDebuffs && !npc.HasBuff(type)) {
        //            for (int i = 0; i < NPC.maxBuffs; i++) {
        //                if (npc.buffTime[i] > 0 && npc.buffType[i] > 0 && IsDebuff(npc.buffType[i])) {
        //                    npc.buffTime[i] = Math.Max(npc.buffTime[i], 180);
        //                }
        //            }
        //        }
        //        if (aequus.soulCrystalDamage > npc.Aequus().debuffDamage) {
        //            npc.Aequus().debuffDamage = (byte)aequus.soulCrystalDamage;
        //            if (Main.netMode != NetmodeID.SinglePlayer) {
        //                var p = Aequus.GetPacket(PacketType.SendDebuffFlatDamage);
        //                p.Write(npc.whoAmI);
        //                p.Write((byte)aequus.soulCrystalDamage);
        //                p.Send();
        //            }
        //        }
        //    }
        //}
        orig(npc, type, time, quiet);
        onAddBuff?.PostAddBuff(npc, time, quiet);
    }

    private static void Player_AddBuff(On_Player.orig_AddBuff orig, Player player, int type, int timeToAdd, bool quiet, bool foodHack) {
        var onAddBuff = BuffLoader.GetBuff(type) as IOnAddBuff;
        onAddBuff?.PreAddBuff(player, ref timeToAdd, ref quiet, ref foodHack);
        //if (BuffSets.BuffConflicts.TryGetValue(EmpoweredBuffBase.GetDepoweredBuff(type), out var l) && l != null) {
        //    for (int i = 0; i < Player.MaxBuffs; i++) {
        //        if (l.Contains(EmpoweredBuffBase.GetDepoweredBuff(player.buffType[i]))) {
        //            player.DelBuff(i);
        //        }
        //    }
        //}
        orig(player, type, timeToAdd, quiet, foodHack);
        onAddBuff?.PostAddBuff(player, timeToAdd, quiet, foodHack);
    }
    #endregion
}