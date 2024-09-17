namespace Aequus.Common.NPCs.Global;

public class FlawlessGlobalNPC : GlobalNPC {
    public override bool InstancePerEntity => true;

    public bool anyInteractedPlayersAreDamaged;
    public bool[] damagedPlayers;
    public bool preventNoHitCheck;

    public FlawlessGlobalNPC() {
        damagedPlayers = new bool[Main.maxPlayers];
    }

    public override void ResetEffects(NPC npc) {
        if (!preventNoHitCheck) {
            foreach (byte p in FlawlessFightSystem.DamagedPlayers) {
                damagedPlayers[p] = true;
                anyInteractedPlayersAreDamaged = true;
            }
        }
    }

    public void ResetNoHit(int player) {
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active)
                Main.npc[i].GetGlobalNPC<FlawlessGlobalNPC>().damagedPlayers[player] = false;
        }
    }

    public static bool HasBeenNoHit(NPC npc, int player) {
        return HasBeenNoHit(npc, npc.GetGlobalNPC<FlawlessGlobalNPC>(), player);
    }

    public static bool HasBeenNoHit(NPC npc, FlawlessGlobalNPC noHit, int player) {
        return npc.playerInteraction[player] && !noHit.damagedPlayers[player];
    }

    #region Mod Calls
    [ModCall("Flawless")]
    public static bool GetPlayerFlawlessFlag(NPC npc, Player player) {
        if (!npc.TryGetGlobalNPC(out FlawlessGlobalNPC flawless)) {
            return false;
        }

        return flawless.damagedPlayers[player.whoAmI];
    }

    [ModCall("FlawlessCheck")]
    public static bool GetNoHitCheckFlag(NPC npc) {
        if (!npc.TryGetGlobalNPC(out FlawlessGlobalNPC flawless)) {
            return false;
        }

        return flawless.preventNoHitCheck;
    }

    // May return null.
    [ModCall("FlawlessStat")]
    public static bool[]? GetPlayerFlawlessArray(NPC npc) {
        if (!npc.TryGetGlobalNPC(out FlawlessGlobalNPC flawless)) {
            return null;
        }

        return flawless.damagedPlayers;
    }
    #endregion
}
