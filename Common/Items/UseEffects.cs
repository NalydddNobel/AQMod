using Aequus.Core.CodeGeneration;

namespace Aequus.Common.Items;

[PlayerGen.Field<bool>("forceUseItem")]
[PlayerGen.Field<byte>("disableItem")]
public class UseEffects : GlobalItem {
    public override bool? UseItem(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0 ? null : false;
    }

    public override bool CanShoot(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0;
    }

    [PlayerGen.SetControls]
    internal static void ForceItemUse(Player player, AequusPlayer aequusPlayer) {
        if (aequusPlayer.forceUseItem) {
            player.controlUseItem = true;
            player.releaseUseItem = true;
            player.itemAnimation = 0;
        }
        aequusPlayer.forceUseItem = false;
    }
}