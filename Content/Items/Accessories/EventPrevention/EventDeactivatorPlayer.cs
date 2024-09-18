using Aequus.Common.DataSets;
using Aequus.Common.Utilities;
using Aequus.Projectiles;

namespace Aequus.Content.Items.Accessories.EventPrevention;

public class EventDeactivatorPlayer : ModPlayer {
    public bool accDisableBloodMoon;
    public bool accDisableEclipse;
    public bool accDisableGlimmer;
    public bool accDisableFrostMoon;
    public bool accDisablePumpkinMoon;

    public override void PreUpdate() {
        CheckFlagOverrides();
    }

    public override bool PreItemCheck() {
        UndoPlayerFlagOverrides();
        return true;
    }

    public override void PostItemCheck() {
        CheckFlagOverrides();
    }

    public override void PostUpdate() {
        UndoPlayerFlagOverrides();
    }

    private void CheckFlagOverrides() {
        if (accDisableBloodMoon) {
            Instance<CacheProps>().BloodMoon.Overwrite(false);
        }
        if (accDisableEclipse) {
            Instance<CacheProps>().Eclipse.Overwrite(false);
        }
        if (accDisablePumpkinMoon) {
            Instance<CacheProps>().PumpkinMoon.Overwrite(false);
        }
        if (accDisableFrostMoon) {
            Instance<CacheProps>().FrostMoon.Overwrite(false);
        }
    }

    public static void CheckPlayerFlagOverrides(Player player) {
        if (player.TryGetModPlayer(out EventDeactivatorPlayer eventDeactivator)) {
            eventDeactivator.CheckFlagOverrides();
        }
    }

    public static void UndoPlayerFlagOverrides() {
        Instance<CacheProps>().BloodMoon.UndoOverwrite();
        Instance<CacheProps>().Eclipse.UndoOverwrite();
        Instance<CacheProps>().PumpkinMoon.UndoOverwrite();
        Instance<CacheProps>().FrostMoon.UndoOverwrite();
    }

    public override void ResetEffects() {
        accDisableBloodMoon = false;
        accDisableGlimmer = false;
        accDisableEclipse = false;
        accDisablePumpkinMoon = false;
        accDisableFrostMoon = false;
    }

    public override void PostUpdateEquips() {
        if (accDisableBloodMoon) {
            foreach (int npc in NPCSets.BloodMoon) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
        if (accDisableGlimmer) {
            foreach (int npc in NPCSets.Glimmer) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
        if (accDisableEclipse) {
            foreach (int npc in NPCSets.Eclipse) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
    }

    public override bool CanHitNPC(NPC target) {
        return (!accDisableBloodMoon || !NPCSets.BloodMoon.Contains(target.netID))
            && (!accDisableGlimmer || !NPCSets.Glimmer.Contains(target.netID))
            && (!accDisableEclipse || !NPCSets.Eclipse.Contains(target.netID));
    }

    public override bool CanBeHitByProjectile(Projectile proj) {
        return proj.TryGetGlobalProjectile(out AequusProjectile source) && source.HasNPCOwner
            ? CanHitNPC(Main.npc[source.sourceNPC])
            : true;
    }
}
