using Aequus.Common.Backpacks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.UI;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void On_ChestUI_QuickStack(On_ChestUI.orig_QuickStack orig, ContainerTransferContext context, bool voidStack) {
        orig(context, voidStack);
        if (voidStack || !Main.LocalPlayer.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }

        var player = Main.LocalPlayer;
        var containerWorldPosition = context.GetContainerWorldPosition();

        var chest = player.GetCurrentChest();
        bool anyTransfers = false;
        if (chest != null) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsQuickStack) {
                    continue;
                }
                BackpackLoader.QuickStack(backpackPlayer.backpacks[i], chest, containerWorldPosition, context);
            }
        }

        if (anyTransfers) {
            SoundEngine.PlaySound(SoundID.Grab);
        }
    }
}
