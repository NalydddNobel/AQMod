namespace Aequus.Common.Buffs.Components;

internal interface IOnAddBuff {
    void PreAddBuff(NPC npc, ref System.Int32 duration, ref System.Boolean quiet) {
    }
    void PostAddBuff(NPC npc, System.Int32 duration, System.Boolean quiet) {
    }

    void PreAddBuff(Player player, ref System.Int32 duration, ref System.Boolean quiet, ref System.Boolean foodHack) {
    }
    void PostAddBuff(Player player, System.Int32 duration, System.Boolean quiet, System.Boolean foodHack) {
    }
}