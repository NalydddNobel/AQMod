using System;

namespace Aequus.Content.Fishing.FishingPoles;

public class FishingPoleGlobalProjectile : GlobalProjectile {
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
        return entity.bobber;
    }

    public override void Load() {
        try {
            DetourHelper.AddHook(typeof(ProjectileLoader), typeof(FishingPoleGlobalProjectile), nameof(ProjectileLoader.ModifyFishingLine));
        }
        catch (Exception ex) {
            // Unimportant if this hook fails loading.
            Mod.Logger.Error(ex);
        }
    }

    public override bool InstancePerEntity => true;

    #region Hooks
    private delegate void ProjectileLoader_ModifyFishingLine_orig(Projectile projectile, ref float polePosX, ref float polePosY, ref Color lineColor);
    private static void ProjectileLoader_ModifyFishingLine(ProjectileLoader_ModifyFishingLine_orig orig, Projectile projectile, ref float polePosX, ref float polePosY, ref Color lineColor) {
        if (Main.player[projectile.owner].HeldItem.ModItem is ModFishingPole fishingPole) {
            fishingPole.GetDrawData(projectile, ref polePosX, ref polePosY, ref lineColor);
        }
        orig(projectile, ref polePosX, ref polePosY, ref lineColor);
    }
    #endregion

    public override bool PreAI(Projectile projectile) {
        return Main.player[projectile.owner].HeldItem.ModItem is ModFishingPole fishingPole ? fishingPole.BobberPreAI(projectile) : true;
    }

    public override void OnKill(Projectile projectile, int timeLeft) {
        if (Main.player[projectile.owner].HeldItem.ModItem is ModFishingPole fishingPole) {
            fishingPole.BobberOnKill(projectile, timeLeft);
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor) {
        return Main.player[projectile.owner].HeldItem.ModItem is ModFishingPole fishingPole ? fishingPole.BobberPreDraw(projectile, ref lightColor) : true;
    }
}