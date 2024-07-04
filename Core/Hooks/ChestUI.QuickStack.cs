using AequusRemake.Content.Backpacks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.UI;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows backpacks to quick stack to nearby chests.</summary>
    private static void On_ChestUI_QuickStack(On_ChestUI.orig_QuickStack orig, ContainerTransferContext context, bool voidStack) {
        orig(context, voidStack);

        if (voidStack || !Main.LocalPlayer.TryGetModPlayer(out BackpackPlayer backpackPlayer)) {
            return;
        }

        Player player = Main.LocalPlayer;
        Vector2 containerWorldPosition = context.GetContainerWorldPosition();

        Chest chest = player.GetCurrentChest();
        bool anyTransfers = false;

        if (chest != null) {
            for (int i = 0; i < backpackPlayer.backpacks.Length; i++) {
                if (!backpackPlayer.backpacks[i].IsActive(player) || !backpackPlayer.backpacks[i].SupportsQuickStack) {
                    continue;
                }

                anyTransfers |= BackpackLoader.QuickStack(backpackPlayer.backpacks[i], chest, containerWorldPosition, context);
            }
        }

        if (anyTransfers) {
            SoundEngine.PlaySound(SoundID.Grab);
        }
    }
}
