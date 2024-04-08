using Aequus.Common.Backpacks;
using Aequus.Common.Golfing;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void On_Player_GetPreferredGolfBallToUse(On_Player.orig_GetPreferredGolfBallToUse orig, Player player, out int projType) {
        orig(player, out projType);
        var heldItem = player.HeldItem;
        if (!heldItem.IsAir && heldItem.shoot == projType) {
            return;
        }

        if (!heldItem.IsAir && (ProjectileLoader.GetProjectile(heldItem.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, heldItem, projType) == true) {
            projType = heldItem.shoot;
            return;
        }

        for (int i = 0; i < Main.InventorySlotsTotal; i++) {
            var item = player.inventory[i];
            if (!item.IsAir && (ProjectileLoader.GetProjectile(item.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, item, projType) == true) {
                projType = item.shoot;
            }
        }

        BackpackLoader.GetPreferredGolfBallToUse(player, player.GetModPlayer<BackpackPlayer>().backpacks, ref projType);
    }
}
