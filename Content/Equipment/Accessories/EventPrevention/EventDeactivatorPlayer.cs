using Aequus.Common.Projectiles;
using Aequus.Core;
using Aequus.DataSets;

namespace Aequus.Content.Equipment.Accessories.EventPrevention;

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
            CommonRefManipulators.BloodMoon.OverrideValue(false);
        }
        if (accDisableEclipse) {
            CommonRefManipulators.Eclipse.OverrideValue(false);
        }
        if (accDisablePumpkinMoon) {
            CommonRefManipulators.PumpkinMoon.OverrideValue(false);
        }
        if (accDisableFrostMoon) {
            CommonRefManipulators.FrostMoon.OverrideValue(false);
        }
    }

    public static void CheckPlayerFlagOverrides(Player player) {
        if (player.TryGetModPlayer(out EventDeactivatorPlayer eventDeactivator)) {
            eventDeactivator.CheckFlagOverrides();
        }
    }

    public static void UndoPlayerFlagOverrides() {
        CommonRefManipulators.BloodMoon.VoidOverriddenValue();
        CommonRefManipulators.Eclipse.VoidOverriddenValue();
        CommonRefManipulators.PumpkinMoon.VoidOverriddenValue();
        CommonRefManipulators.FrostMoon.VoidOverriddenValue();
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
            foreach (int npc in NPCDataSet.FromBloodMoon) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
        if (accDisableGlimmer) {
            foreach (int npc in NPCDataSet.FromGlimmer) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
        if (accDisableEclipse) {
            foreach (int npc in NPCDataSet.FromEclipse) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
    }

    public override bool CanHitNPC(NPC target) {
        return (!accDisableBloodMoon || !NPCDataSet.FromBloodMoon.Contains(target.netID))
            && (!accDisableGlimmer || !NPCDataSet.FromGlimmer.Contains(target.netID))
            && (!accDisableEclipse || !NPCDataSet.FromEclipse.Contains(target.netID));
    }

    public override bool CanBeHitByProjectile(Projectile proj) {
        return proj.TryGetGlobalProjectile(out ProjectileSource source) && source.HasNPCOwner
            ? CanHitNPC(Main.npc[source.parentNPCIndex])
            : true;
    }
}
