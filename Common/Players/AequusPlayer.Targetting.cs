using Aequus.Core.Generator;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>
    /// The closest 'enemy' NPC to the player. Updated in <see cref="PostUpdate"/>
    /// </summary>
    [ResetEffects(-1)]
    public System.Int32 closestEnemy;
    [ResetEffects(-1)]
    public System.Int32 closestEnemyOld;

    /// <summary>
    /// Helper for whether or not the player is in danger
    /// </summary>
    public System.Boolean InDanger => closestEnemy != -1;

    private void UpdateDangers() {
        System.Boolean safe = closestEnemy == -1;
        closestEnemyOld = closestEnemy;
        closestEnemy = -1;

        var center = Player.Center;
        var checkTangle = new Rectangle((System.Int32)Player.position.X + Player.width / 2 - 1000, (System.Int32)Player.position.Y + Player.height / 2 - 500, 2000, 1000);
        System.Single distance = 1000f;
        for (System.Int32 i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type != NPCID.TargetDummy && Main.npc[i].CanBeChasedBy(Player) && !Main.npc[i].IsProbablyACritter()) {
                System.Single d = Main.npc[i].Distance(center);
                if (d < distance) {
                    distance = d;
                    closestEnemy = i;
                }
            }
        }
    }
}