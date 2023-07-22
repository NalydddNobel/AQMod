using Terraria;

namespace Aequus.NPCs.BossMonsters.Crabson.Common;

public class CrabsonMood {
    public int laughAnimation;
    public int timeSinceDamagedPlayer;
    public int damageRecievedSinceDamagedPlayer;

    public void OnAIUpdate() {
        damageRecievedSinceDamagedPlayer++;
    }

    public void Laugh() {
        laughAnimation = 60;
        timeSinceDamagedPlayer = 0;
        damageRecievedSinceDamagedPlayer = 0;
    }

    public void OnDamageRecieved(NPC.HitInfo hit) {
        damageRecievedSinceDamagedPlayer += hit.Damage;
    }
}