using Aequus.Common.Projectiles;
using Aequus.Content.DataSets;
using Aequus.Core;

namespace Aequus;

public class EventDeactivatorPlayer : ModPlayer {
    public bool accDisableBloodMoon;
    public bool accDisableEclipse;
    public bool accDisableGlimmer;

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
    }

    public static void CheckPlayerFlagOverrides(Player player) {
        if (player.TryGetModPlayer(out EventDeactivatorPlayer eventDeactivator)) {
            eventDeactivator.CheckFlagOverrides();
        }
    }

    public static void UndoPlayerFlagOverrides() {
        CommonRefManipulators.BloodMoon.VoidOverriddenValue();
        CommonRefManipulators.Eclipse.VoidOverriddenValue();
    }

    public override void ResetEffects() {
        accDisableBloodMoon = false;
        accDisableGlimmer = false;
        accDisableEclipse = false;
    }

    public override void PostUpdateEquips() {
        if (accDisableBloodMoon) {
            foreach (int npc in NPCMetadata.FromBloodMoon) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
        if (accDisableGlimmer) {
            foreach (int npc in NPCMetadata.FromGlimmer) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
        if (accDisableEclipse) {
            foreach (int npc in NPCMetadata.FromEclipse) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
    }

    public override bool CanHitNPC(NPC target) {
        return (!accDisableBloodMoon || !NPCMetadata.FromBloodMoon.Contains(target.netID))
            && (!accDisableGlimmer || !NPCMetadata.FromGlimmer.Contains(target.netID))
            && (!accDisableEclipse || !NPCMetadata.FromEclipse.Contains(target.netID));
    }

    public override bool CanBeHitByProjectile(Projectile proj) {
        return proj.TryGetGlobalProjectile(out ProjectileSource source) && source.HasNPCOwner
            ? CanHitNPC(Main.npc[source.parentNPCIndex])
            : true;
    }
}
