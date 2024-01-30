namespace Aequus.Common.Buffs.Components;

internal interface IOnAddBuff {
    void PreAddBuff(NPC npc, ref int duration, ref bool quiet) {
    }
    void PostAddBuff(NPC npc, int duration, bool quiet) {
    }

    void PreAddBuff(Player player, ref int duration, ref bool quiet, ref bool foodHack) {
    }
    void PostAddBuff(Player player, int duration, bool quiet, bool foodHack) {
    }
}