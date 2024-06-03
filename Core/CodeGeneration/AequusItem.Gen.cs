
namespace Aequus;

public partial class AequusItem : GlobalItem {
    public override bool? UseItem(Item item, Player player) {
#if !DEBUG
        UseItemInner(item, player, player.GetModPlayer<AequusPlayer>());
#endif
        return null;
    }
}
