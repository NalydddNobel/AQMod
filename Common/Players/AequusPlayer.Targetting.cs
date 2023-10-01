using Aequus.Common.Players.Attributes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>
    /// The closest 'enemy' NPC to the player. Updated in <see cref="PostUpdate"/>
    /// </summary>
    [ResetEffects(-1)]
    public int closestEnemy;
    [ResetEffects(-1)]
    public int closestEnemyOld;

    /// <summary>
    /// Helper for whether or not the player is in danger
    /// </summary>
    public bool InDanger => closestEnemy != -1;

    private void UpdateDangers() {
        bool safe = closestEnemy == -1;
        closestEnemyOld = closestEnemy;
        closestEnemy = -1;

        var center = Player.Center;
        var checkTangle = new Rectangle((int)Player.position.X + Player.width / 2 - 1000, (int)Player.position.Y + Player.height / 2 - 500, 2000, 1000);
        float distance = 1000f;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy && Main.npc[i].CanBeChasedBy(Player) && !Main.npc[i].IsProbablyACritter()) {
                float d = Main.npc[i].Distance(center);
                if (d < distance) {
                    distance = d;
                    closestEnemy = i;
                }
            }
        }
    }
}