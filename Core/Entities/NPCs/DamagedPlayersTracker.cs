namespace AequusRemake.Core.Entities.NPCs;

public class DamagedPlayersTracker : GlobalNPC {
    public bool anyInteractedPlayersAreDamaged;

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override void PostAI(NPC npc) {
        if (anyInteractedPlayersAreDamaged) { return; }

        for (int i = 0; i < Main.maxPlayers; i++) {
            if (!npc.playerInteraction[i]) {
                continue;
            }

            Player player = Main.player[i];
            if (!player.active) {
                continue;
            }

            if (player.statLife < player.statLifeMax) {
                anyInteractedPlayersAreDamaged = true;
            }
        }
    }
}
