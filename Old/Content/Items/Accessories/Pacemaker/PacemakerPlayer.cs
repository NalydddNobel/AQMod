using System;

namespace Aequus.Old.Content.Items.Accessories.Pacemaker;

public class PacemakerPlayer : ModPlayer {
    public int respawnSpeedupTime;

    public override void ResetEffects() {
        respawnSpeedupTime = 0;
    }

    public override void UpdateDead() {
        if (respawnSpeedupTime == 0) {
            return;
        }

        if (respawnSpeedupTime < 180) {
            respawnSpeedupTime++;
            return;
        }

        if (NPC.AnyDanger(quickBossNPCCheck: true, ignorePillarsAndMoonlordCountdown: false)) {
            return;
        }

        var spawnTile = Player.GetSpawn();
        var spawnCoords = spawnTile.ToWorldCoordinates();
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].lifeMax > 5 && Main.npc[i].damage > 0) {
                if (Vector2.Distance(spawnCoords, Main.npc[i].Center) < 800f) {
                    return;
                }
            }
        }

        if (Player.respawnTimer > 5) {
            Player.respawnTimer = Math.Max(Player.respawnTimer - 4, 5);
        }
    }
}
