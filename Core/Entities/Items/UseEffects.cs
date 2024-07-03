using Aequu2.Core.CodeGeneration;

namespace Aequu2.Core.Entities.Items;

[Gen.AequusPlayer_Field<bool>("forceUseItem")]
[Gen.AequusPlayer_Field<byte>("disableItem")]
public class UseEffects : GlobalItem {
    public override bool? UseItem(Item item, Player player) {
        if (player.GetModPlayer<AequusPlayer>().disableItem == 0) {
            return null;
        }
        player.itemTime = player.itemAnimation;
        return false;
    }

    public override bool CanShoot(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0;
    }

    [Gen.AequusPlayer_SetControls]
    internal static void ForceItemUse(Player player, AequusPlayer Aequu2Player) {
        if (Aequu2Player.forceUseItem) {
            player.controlUseItem = true;
            player.releaseUseItem = true;
            player.itemAnimation = 0;
        }
        Aequu2Player.forceUseItem = false;
    }
}