using Aequus.Common.Backpacks;
using Aequus.Common.Golfing;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for custom golf ball projectiles to be picked by this method. And allows golf balls in backpacks to be chosen aswell.</summary>
    private static void On_Player_GetPreferredGolfBallToUse(On_Player.orig_GetPreferredGolfBallToUse orig, Player player, out int projType) {
        orig(player, out projType);

        Item heldItem = player.HeldItem;
        if (!heldItem.IsAir && heldItem.shoot == projType) {
            return;
        }

        if (!heldItem.IsAir && (ProjectileLoader.GetProjectile(heldItem.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, heldItem, projType) == true) {
            projType = heldItem.shoot;
            return;
        }

        for (int i = 0; i < Main.InventorySlotsTotal; i++) {
            Item item = player.inventory[i];
            if (!item.IsAir && (ProjectileLoader.GetProjectile(item.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, item, projType) == true) {
                projType = item.shoot;
            }
        }

        BackpackLoader.GetPreferredGolfBallToUse(player, player.GetModPlayer<BackpackPlayer>().backpacks, ref projType);
    }
}
