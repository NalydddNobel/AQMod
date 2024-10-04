using Aequus.Projectiles;

namespace Aequus.Content.Items.Accessories.AccessoryDamageEarring;

public class AccessoryDamageGlobalProjectile : GlobalProjectile {
    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
        // Is the projectile friendly?
        if (!projectile.friendly || !Main.player[projectile.owner].TryGetModPlayer(out AequusPlayer aequusPlayer)) {
            return;
        }

        // Check if the projectile comes from an item.
        if (!projectile.TryGetGlobalProjectile(out AequusProjectile aequusProj) || !aequusProj.FromItem) {
            return;
        }

        // Check if the item has a damage stat.
        // If the item does indeed have a damage stat, we shoul
        if (!ContentSamples.ItemsByType.TryGetValue(aequusProj.sourceItemUsed, out Item? referenceItem)) {
            return;
        }

        // Check the item source if it's an Accessory.

        // Also check if the accessory has a damage stat...
        // If it does, it most likely has a damage boost already.
        if (!referenceItem.accessory || referenceItem.damage > 0) {
            return;
        }

        // Apply the bonus here...
        // Some accessories like Spore Sac do not caluclate damage from the item.
        modifiers.SourceDamage = modifiers.SourceDamage.CombineWith(aequusPlayer.accessoryDamage);
    }
}