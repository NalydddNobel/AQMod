namespace Aequu2.Core.Components.Buffs;

internal interface IOnAddBuff {
    void PreAddBuff(NPC npc, bool alreadyHasBuff, ref int duration, ref bool quiet) {
    }
    void PostAddBuff(NPC npc, bool alreadyHasBuff, int duration, bool quiet) {
    }

    void PreAddBuff(Player player, bool alreadyHasBuff, ref int duration, ref bool quiet, ref bool foodHack) {
    }
    void PostAddBuff(Player player, bool alreadyHasBuff, int duration, bool quiet, bool foodHack) {
    }
}