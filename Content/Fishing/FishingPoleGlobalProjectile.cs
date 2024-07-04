using AequusRemake.Core.ContentGeneration;
using System;

namespace AequusRemake.Content.Fishing;

public class FishingPoleGlobalProjectile : GlobalProjectile {
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
        return entity.bobber;
    }

    public override bool InstancePerEntity => true;

    public override bool PreAI(Projectile projectile) {
        projectile.ai[1] = Math.Max(-360f, projectile.ai[1]);
        return Main.player[projectile.owner].HeldItem.ModItem is UnifiedFishingPole fishingPole ? fishingPole.BobberPreAI(projectile) : true;
    }

    public override void OnKill(Projectile projectile, int timeLeft) {
        if (Main.player[projectile.owner].HeldItem.ModItem is UnifiedFishingPole fishingPole) {
            fishingPole.BobberOnKill(projectile, timeLeft);
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor) {
        return Main.player[projectile.owner].HeldItem.ModItem is UnifiedFishingPole fishingPole ? fishingPole.BobberPreDraw(projectile, ref lightColor) : true;
    }
}